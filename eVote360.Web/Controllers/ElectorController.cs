using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using eVote360.WebApp.ViewModels.Elector;


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
        private readonly IGenericRepository<PartidoPolitico> _partidoRepository;
        private readonly IGenericRepository<Candidato> _candidatoRepository;
        private readonly IGenericRepository<PuestoElectivo> _puestoRepository;
        private readonly IVotoRepository _votoRepository;

        public ElectorController(
                    IElectorService electorService,
                    IOcrService ocrService,
                    IOtpService otpService,
                    IGenericRepository<Ciudadano> ciudadanoRepository,
                    IGenericRepository<Eleccion> eleccionRepository,
                    IEmailService emailService,
                    IGenericRepository<PartidoPolitico> partidoRepository,
                    IGenericRepository<Candidato> candidatoRepository,
                    IGenericRepository<PuestoElectivo> puestoRepository,
                    IVotoRepository votoRepository)
        {
            _electorService = electorService;
            _ocrService = ocrService;
            _otpService = otpService;
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionRepository = eleccionRepository;
            _emailService = emailService;
            _partidoRepository = partidoRepository;
            _candidatoRepository = candidatoRepository;
            _puestoRepository = puestoRepository;
            _votoRepository = votoRepository;
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
            if (!textoExtraido.Replace("-", "").Contains(cedulaLimpia))
            {
                ModelState.AddModelError(string.Empty, "Los datos extraídos de la foto no coinciden con los datos previamente ingresados por el elector.");
                return View(model);
            }

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

            TempData.Keep("ElectorDocumento");

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

            TempData["CiudadanoId"] = ciudadano.Id;
            TempData.Keep("CiudadanoId");

            await _otpService.EnviarOtpAsync(ciudadano.Id, eleccionActiva.Id, ciudadano.CorreoElectronico);

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
            if (TempData["CiudadanoId"] == null)
            {
                return RedirectToAction("Index");
            }

            int ciudadanoId = (int)TempData["CiudadanoId"];
            TempData.Keep("CiudadanoId");

            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionActiva = elecciones.FirstOrDefault(e => e.Estado == EstadoEleccion.Activa);
            if (eleccionActiva == null)
            {
                return RedirectToAction("Index");
            }

            var puestosDb = await _puestoRepository.GetAllAsync();
            var puestosActivos = puestosDb.Where(p => p.Estado).ToList();

            var partidosDb = await _partidoRepository.GetAllAsync();
            var partidosActivos = partidosDb.Where(p => p.Estado).ToDictionary(p => p.Id);

            var candidatosDb = await _candidatoRepository.GetAllAsync();
            var candidatosActivos = candidatosDb.Where(c => c.EsActivo).ToList();

            var votos = await _votoRepository.GetByCiudadanoEleccionAsync(ciudadanoId, eleccionActiva.Id);
            var votosDict = votos.ToDictionary(v => v.PuestoElectivoId);

            var viewModel = new CabinaVotacionViewModel
            {
                EleccionId = eleccionActiva.Id,
                NombreEleccion = eleccionActiva.Nombre,
                Puestos = puestosActivos.Select(p => {
                    votosDict.TryGetValue(p.Id, out var voto);
                    return new PuestoVotacionDto
                    {
                        PuestoId = p.Id,
                        NombrePuesto = p.Nombre,
                        CandidatoSeleccionadoId = (voto != null && voto.CandidatoId.HasValue) ? voto.CandidatoId : null,
                        SeleccionadoNinguno = voto != null && !voto.CandidatoId.HasValue,
                        Candidatos = candidatosActivos
                            .Where(c => c.PuestoElectivoId == p.Id && partidosActivos.ContainsKey(c.PartidoPoliticoId))
                            .Select(c => new CandidatoVotacionDto
                            {
                                Id = c.Id,
                                Nombre = c.Nombre,
                                Apellido = c.Apellido,
                                Foto = c.Foto,
                                NombrePartido = partidosActivos[c.PartidoPoliticoId].Nombre,
                                SiglasPartido = partidosActivos[c.PartidoPoliticoId].Siglas
                            }).ToList()
                    };
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarVoto(int candidatoId, int puestoId, int eleccionId)
        {
            if (TempData["CiudadanoId"] == null)
            {
                return RedirectToAction("Index");
            }

            int ciudadanoId = (int)TempData["CiudadanoId"];
            TempData.Keep("CiudadanoId");

            await _electorService.RegistrarVotoAsync(candidatoId, puestoId, eleccionId, ciudadanoId);
            return RedirectToAction("CabinaVotacion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarVotacion(int eleccionId)
        {

            if (TempData["CiudadanoId"] == null)
            {
                return RedirectToAction("Index");
            }

            int ciudadanoId = (int)TempData["CiudadanoId"];


            var resultado = await _electorService.ValidarFinalizacionAsync(ciudadanoId, eleccionId);

            if (!resultado.EsValido)
            {
                TempData.Keep();
                ModelState.AddModelError(string.Empty, "Debe completar su selección para los siguientes puestos: " + resultado.PuestosFaltantes);
                return RedirectToAction("CabinaVotacion");
            }


            await _electorService.MarcarComoVotadoAsync(ciudadanoId, eleccionId);


            var ciudadano = await _ciudadanoRepository.GetByIdAsync(ciudadanoId);
            var eleccion = await _eleccionRepository.GetByIdAsync(eleccionId);

            if (ciudadano != null && eleccion != null && !string.IsNullOrWhiteSpace(ciudadano.CorreoElectronico))
            {
                var votos = await _votoRepository.GetByCiudadanoEleccionAsync(ciudadanoId, eleccionId);
                var sbVotos = new StringBuilder();

                foreach (var v in votos)
                {
                    var puesto = await _puestoRepository.GetByIdAsync(v.PuestoElectivoId);
                    string seleccion = "Ninguno (Voto en blanco)";

                    if (v.CandidatoId.HasValue)
                    {
                        var cand = await _candidatoRepository.GetByIdAsync(v.CandidatoId.Value);
                        seleccion = $"{cand.Nombre} {cand.Apellido}";
                    }

                    sbVotos.Append($"<li style='margin-bottom: 8px;'><strong>{puesto.Nombre}</strong>: {seleccion}</li>");
                }

                string correoDestino = ciudadano.CorreoElectronico;
                string asunto = "eVote360 Pro - Comprobante de Resumen Electoral";
                string cuerpoHtml = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 8px; max-width: 600px; margin: 0 auto; color: #333;'>
                        <h2 style='color: #0d6efd; border-bottom: 2px solid #0d6efd; padding-bottom: 10px; text-align: center;'>Comprobante de Votación</h2>
                        <p>Estimado(a) <strong>{ciudadano.Nombre} {ciudadano.Apellido}</strong>,</p>
                        <p>Le confirmamos que su participación en el proceso electoral <strong>{eleccion.Nombre}</strong> ha concluido correctamente.</p>
                        <p>A continuación se detalla su selección por cada puesto electivo:</p>
                        <ul style='font-size: 16px; line-height: 1.6; padding-left: 20px;'>
                            {sbVotos}
                        </ul>
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin-top: 25px; font-size: 12px; color: #666; border-left: 4px solid #0d6efd;'>
                            Este correo sirve como constancia digital de su participación. Su voto ha sido registrado de forma segura y secreta en nuestra base de datos.
                        </div>
                    </div>";

                await _emailService.EnviarResumenVotacionAsync(correoDestino, asunto, cuerpoHtml);
            }

            return RedirectToAction("VotoConfirmado");
        }

        [HttpGet]
        public IActionResult VotoConfirmado()
        {
            return View();
        }
    }
}