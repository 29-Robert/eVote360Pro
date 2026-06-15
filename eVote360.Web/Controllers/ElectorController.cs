using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace eVote360.WebApp.Controllers
{
    public class ElectorController : Controller
    {
        private readonly IElectorService _electorService;
        private readonly IOcrService _ocrService;
        private readonly IOtpService _otpService;
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepository;
        private readonly IGenericRepository<Eleccion> _eleccionRepository;
        public ElectorController(
                    IElectorService electorService,
                    IOcrService ocrService,
                    IOtpService otpService,
                    IGenericRepository<Ciudadano> ciudadanoRepository,
                    IGenericRepository<Eleccion> eleccionRepository)
        {
            _electorService = electorService;
            _ocrService = ocrService;
            _otpService = otpService;
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionRepository = eleccionRepository;
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
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            try
            {
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
            if (TempData["ElectorDocumento"] == null)
            {
                return RedirectToAction("Index");
            }

            TempData.Keep();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarOcr(IFormFile fotoCedula)
        {
            if (fotoCedula == null || fotoCedula.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Por favor, seleccione una foto válida de su cédula.");
                return View();
            }

            string? cedulaEsperada = TempData["ElectorDocumento"] as string;
            if (string.IsNullOrEmpty(cedulaEsperada))
            {
                return RedirectToAction("Index");
            }

            try
            {
                TempData.Keep();

                using (var stream = fotoCedula.OpenReadStream())
                {
                    string textoExtraido = await _ocrService.ExtraerTextoDeImagenAsync(stream);

                    string cedulaLimpiaEsperada = Regex.Replace(cedulaEsperada, @"[^\d]", "");

                    string textoLimpioOcr = Regex.Replace(textoExtraido, @"[^\d]", "");

                    bool coinciden = textoLimpioOcr.Contains(cedulaLimpiaEsperada);

                    if (!coinciden)
                    {
                        ModelState.AddModelError(string.Empty, "La cédula detectada en la imagen no coincide con el documento de identidad digitado inicialmente.");
                        return View();
                    }
                }

                return RedirectToAction("EnviarCodigoOtp");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al procesar el escaneo del documento: {ex.Message}");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EnviarCodigo()
        {
            string? cedula = TempData["ElectorDocumento"] as string;
            if (string.IsNullOrEmpty(cedula))
            {
                return RedirectToAction("Index");
            }

            TempData.Keep();

            try
            {
                var ciudadanos = await _ciudadanoRepository.GetAllAsync();
                var ciudadano = ciudadanos.FirstOrDefault(c => c.DocumentoIdentidad == cedula);

                var elecciones = await _eleccionRepository.GetAllAsync();
                var eleccionActiva = elecciones.FirstOrDefault(e => e.Estado == EstadoEleccion.Activa);

                if (ciudadano == null || eleccionActiva == null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró un proceso electoral activo o el ciudadano no está registrado.");
                    return RedirectToAction("Index");
                }

                await _otpService.EnviarOtpAsync(ciudadano.Id, eleccionActiva.Id, ciudadano.CorreoElectronico);

                ViewBag.CiudadanoId = ciudadano.Id;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"No se pudo enviar el código de verificación: {ex.Message}");
                return RedirectToAction("ValidarOcr");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarOtp(int ciudadanoId, string codigoOtp)
        {
            if (string.IsNullOrEmpty(codigoOtp))
            {
                ModelState.AddModelError(string.Empty, "Por favor, introduzca el código de verificación.");
                ViewBag.CiudadanoId = ciudadanoId;
                return View("EnviarCodigoOtp");
            }

            try
            {
                TempData.Keep();

                // 4. Validamos el código
                bool esValido = await _otpService.ValidarOtpAsync(ciudadanoId, codigoOtp);

                if (!esValido)
                {
                    ModelState.AddModelError(string.Empty, "El código introducido es incorrecto, ya fue utilizado o ha expirado.");
                    ViewBag.CiudadanoId = ciudadanoId;
                    return View("EnviarCodigoOtp");
                }

                // 5. ÉXITO: El ciudadano se autenticó por completo. Redirige a la Cabina de Votación (Fase 5)
                return RedirectToAction("CabinaVotacion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error durante la verificación: {ex.Message}");
                ViewBag.CiudadanoId = ciudadanoId;
                return View("EnviarCodigoOtp");
            }
        }
    }
}
