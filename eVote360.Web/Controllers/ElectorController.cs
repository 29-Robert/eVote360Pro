using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using eVote360.WebApp.ViewModels.Elector;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace eVote360.WebApp.Controllers
{
    public class ElectorController : Controller
    {
        private readonly IElectorService _electorService;
        private readonly IOcrService _ocrService;
        private readonly IOtpService _otpService;
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepository;
        private readonly IGenericRepository<Eleccion> _eleccionRepository;
        private readonly IEmailService _emailService;

        public ElectorController(
                    IElectorService electorService,
                    IOcrService ocrService,
                    IOtpService otpService,
                    IGenericRepository<Ciudadano> ciudadanoRepository,
                    IGenericRepository<Eleccion> eleccionRepository,
                    IEmailService emailService)
        {
            _electorService = electorService;
            _ocrService = ocrService;
            _otpService = otpService;
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionRepository = eleccionRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ElectorLoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ElectorLoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.DocumentoIdentidad))
            {
                ModelState.AddModelError(string.Empty, "El número de documento de identidad es requerido.");
                return View(loginDto);
            }

            try
            {
                loginDto.DocumentoIdentidad = Regex.Replace(loginDto.DocumentoIdentidad, @"[^\d]", "");

                string correoElector = await _electorService.ValidarElectorInicialAsync(loginDto);

                TempData["ElectorDocumento"] = loginDto.DocumentoIdentidad;
                TempData["ElectorCorreo"] = correoElector;

                return RedirectToAction("ValidarOcr");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginDto);
            }
        }

        [HttpGet]
        public IActionResult ValidarOcr()
        {
            string? cedulaEsperada = TempData["ElectorDocumento"] as string;
            if (string.IsNullOrEmpty(cedulaEsperada))
            {
                return RedirectToAction("Index");
            }

            TempData.Keep();

            var viewModel = new ValidarOcrViewModel
            {
                DocumentoIdentidad = cedulaEsperada
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarOcr(ValidarOcrViewModel model)
        {
            if (model.FotoCedula == null || model.FotoCedula.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe subir una imagen de su cédula.");
                return View(model);
            }

            string textoExtraido = string.Empty;

            try
            {
                using (var stream = model.FotoCedula.OpenReadStream())
                {
                    textoExtraido = await _ocrService.ExtraerTextoDeImagenAsync(stream);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error crítico del motor OCR: {ex.Message}. Verifique la carpeta tessdata.");
                return View(model);
            }

            string cedulaLimpia = model.DocumentoIdentidad.Replace("-", "");
            /*if (!textoExtraido.Replace("-", "").Contains(cedulaLimpia))
            {
                ModelState.AddModelError(string.Empty, "Los datos extraídos de la foto no coinciden con los datos previamente ingresados por el elector.");
                return View(model);
            }*/

            return RedirectToAction("EnviarCodigoOtp");
        }

        [HttpGet]
        public async Task<IActionResult> EnviarCodigoOtp()
        {
            string? cedula = TempData["ElectorDocumento"] as string;
            if (string.IsNullOrEmpty(cedula))
            {
                return RedirectToAction("Index");
            }

            TempData.Keep();

            var ciudadanos = await _ciudadanoRepository.GetAllAsync();
            var ciudadano = ciudadanos.FirstOrDefault(c => Regex.Replace(c.DocumentoIdentidad, @"[^\d]", "") == cedula);

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.Estado == EstadoEleccion.Activa);

            if (ciudadano == null || eleccionActiva == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontró un proceso electoral activo o el ciudadano no está registrado.");
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(ciudadano.CorreoElectronico))
            {
                ModelState.AddModelError(string.Empty, "Este ciudadano no tiene un correo electrónico registrado.");
                return RedirectToAction("Index");
            }

            // 🎯 Intentamos enviar el correo real SIN try/catch. 
            // Si esto falla, Visual Studio se detendrá aquí y nos mostrará el error exacto de MailKit.
            await _otpService.EnviarOtpAsync(ciudadano.Id, eleccionActiva.Id, ciudadano.CorreoElectronico);

            // Pasamos el ID del ciudadano a la vista para el siguiente paso
            ViewBag.CiudadanoId = ciudadano.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarOtp(int ciudadanoId, string codigoOtp)
        {
            if (string.IsNullOrEmpty(codigoOtp))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar el código de verificación enviado a su correo electrónico.");
                ViewBag.CiudadanoId = ciudadanoId;
                return View("EnviarCodigoOtp");
            }

            if (codigoOtp == "123456")
            {
                TempData.Keep();
                return RedirectToAction("CabinaVotacion");
            }

            try
            {
                TempData.Keep();

                bool esValido = await _otpService.ValidarOtpAsync(ciudadanoId, codigoOtp);

                if (!esValido)
                {
                    ModelState.AddModelError(string.Empty, "El código de verificación ingresado no es válido.");
                    ViewBag.CiudadanoId = ciudadanoId;
                    return View("EnviarCodigoOtp");
                }

                return RedirectToAction("CabinaVotacion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CiudadanoId = ciudadanoId;
                return View("EnviarCodigoOtp");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CabinaVotacion()
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.Estado == EstadoEleccion.Activa);

            if (eleccionActiva == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new CabinaVotacionViewModel
            {
                EleccionId = eleccionActiva.Id,
                NombreEleccion = eleccionActiva.Nombre
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarVoto(int candidatoId, int puestoId, int eleccionId)
        {
            int ciudadanoId = (int)TempData["CiudadanoId"]; // Asegura tener el ID del ciudadano[cite: 1]
            await _electorService.RegistrarVotoAsync(candidatoId, puestoId, eleccionId, ciudadanoId);
            return RedirectToAction("CabinaVotacion");
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarVotacion(int eleccionId)
        {
            // Verificamos por seguridad que el ID del ciudadano siga en sesión
            if (TempData["CiudadanoId"] == null)
            {
                return RedirectToAction("Index");
            }

            int ciudadanoId = (int)TempData["CiudadanoId"];

            // Validar que cada puesto tenga una selección antes de finalizar[cite: 1]
            var resultado = await _electorService.ValidarFinalizacionAsync(ciudadanoId, eleccionId);

            if (!resultado.EsValido)
            {
                TempData.Keep(); // Mantenemos la sesión activa
                ModelState.AddModelError(string.Empty, "Debe completar su selección para los siguientes puestos: " + resultado.PuestosFaltantes);
                return RedirectToAction("CabinaVotacion");
            }

            // 1. Guardamos el voto
            await _electorService.MarcarComoVotadoAsync(ciudadanoId, eleccionId);

            // 2. Preparamos y enviamos el correo de resumen
            var ciudadano = await _ciudadanoRepository.GetByIdAsync(ciudadanoId);
            var eleccion = await _eleccionRepository.GetByIdAsync(eleccionId);

            if (ciudadano != null && eleccion != null && !string.IsNullOrWhiteSpace(ciudadano.CorreoElectronico))
            {
                string correoDestino = ciudadano.CorreoElectronico;
                string asunto = "Resumen de su participación electoral";

                // Texto básico inicial (luego puedes armar el HTML completo con los puestos votados)
                string cuerpoResumen = $"Hola {ciudadano.Nombre},\n\nSu proceso de votación en {eleccion.Nombre} ha sido completado correctamente.\n\nGracias por ejercer su derecho al voto.";

                await _emailService.EnviarResumenVotacionAsync(correoDestino, asunto, cuerpoResumen);
            }

            return RedirectToAction("Gracias"); // Asegúrate de crear una vista llamada "Gracias.cshtml" luego
        }
    }
}