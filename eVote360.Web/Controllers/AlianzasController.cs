using eVote360.Core.Application.DTOs.Alianzas;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Alianzas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Dirigente político")]
    public class AlianzasController : Controller
    {
        private readonly IAlianzaRepository alianzaRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IAlianzaService _alianzaService;

        public AlianzasController(IAlianzaRepository alianzaRepo,
                                   IEleccionRepository eleccionRepo,
                                   IAlianzaService alianzaService)
        {
            this.alianzaRepo = alianzaRepo;
            _eleccionRepo = eleccionRepo;
            _alianzaService = alianzaService;
        }

        public async Task<IActionResult> Index()
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var vm = new AlianzasViewModel
            {
                Pendientes = await _alianzaService.GetPendientesAsync(partidoId),
                Realizadas = await _alianzaService.GetRealizadasAsync(partidoId),
                Vigentes = await _alianzaService.GetVigentesAsync(partidoId)
            };
            return View(vm);
        }

        public async Task<IActionResult> CrearSolicitud()
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
            {
                TempData["ErrorMessage"] = "No se pueden modificar alianzas mientras exista una elección activa.";
                return RedirectToAction("Index");
            }

            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var partidos = await alianzaRepo.GetPartidosDisponiblesAsync(partidoId);

            var vm = new CrearSolicitudAlianzaViewModel
            {
                PartidosDisponibles = partidos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} ({p.Siglas})"
                })
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud(CrearSolicitudAlianzaViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
                await _alianzaService.CrearSolicitudAsync(partidoId, vm.PartidoReceptorId);
        
                TempData["SuccessMessage"] = "Solicitud de alianza enviada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Aceptar(int id)
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var solicitud = await alianzaRepo.GetByIdAsync(id);
            if (solicitud == null || solicitud.PartidoReceptorId != partidoId)
            {
                TempData["ErrorMessage"] = "No tiene permisos para responder esta solicitud.";
                return RedirectToAction("Index");
            }
            return View(solicitud);
        }

        [HttpPost]
        public async Task<IActionResult> AceptarConfirmado(int id)
        {
            try
            {
                await _alianzaService.AceptarAsync(id);

                TempData["SuccessMessage"] = "Alianza aceptada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Rechazar(int id)
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var solicitud = await alianzaRepo.GetByIdAsync(id);
            if (solicitud == null || solicitud.PartidoReceptorId != partidoId)
            {
                TempData["ErrorMessage"] = "No tiene permisos para responder esta solicitud.";
                return RedirectToAction("Index");
            }
            return View(solicitud);
        }

        [HttpPost]
        public async Task<IActionResult> RechazarConfirmado(int id)
        {
            try
            {
                await _alianzaService.RechazarAsync(id);
                TempData["SuccessMessage"] = "Solicitud rechazada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var solicitud = await alianzaRepo.GetByIdAsync(id);
            if (solicitud == null || solicitud.PartidoSolicitanteId != partidoId)
            {
                TempData["ErrorMessage"] = "No tiene permisos para eliminar esta solicitud.";
                return RedirectToAction("Index");
            }
            return View(solicitud);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarSolicitudConfirmado(int id)
        {
            try
            {
                await _alianzaService.EliminarSolicitudAsync(id);
                TempData["SuccessMessage"] = "Solicitud eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EliminarAlianza(int id)
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var solicitud = await alianzaRepo.GetByIdAsync(id);
            if (solicitud == null || (solicitud.PartidoSolicitanteId != partidoId && solicitud.PartidoReceptorId != partidoId))
            {
                TempData["ErrorMessage"] = "No tiene permisos para eliminar esta alianza.";
                return RedirectToAction("Index");
            }
            return View(solicitud);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarAlianzaConfirmado(int id)
        {
            try
            {
                await _alianzaService.EliminarAlianzaAsync(id);
                TempData["SuccessMessage"] = "Alianza eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}