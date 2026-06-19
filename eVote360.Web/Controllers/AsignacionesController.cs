using eVote360.Core.Application.DTOs.Asignaciones; // Deberías tener un DTO específico para esto
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Asignaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360.WebApp.Controllers
{
    public class AsignacionesController : Controller
    {
        private readonly ICandidatoPuestoService _asignacionService;
        private readonly IEleccionRepository _eleccionRepo;

        public AsignacionesController(ICandidatoPuestoService asignacionService, IEleccionRepository eleccionRepo)
        {
            _asignacionService = asignacionService;
            _eleccionRepo = eleccionRepo;
        }

        public async Task<IActionResult> Index()
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId")?.Value ?? "0");
            var asignaciones = await _asignacionService.GetByPartidoAsync(partidoId);
            return View(asignaciones);
        }

        public async Task<IActionResult> Agregar()
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
            {
                TempData["Error"] = "No se pueden modificar asignaciones mientras exista una elección activa.";
                return RedirectToAction("Index");
            }

            var partidoId = int.Parse(User.FindFirst("PartidoId")?.Value ?? "0");
            var vm = new AgregarAsignacionesViewModel();

            // Lógica para llenar los selects usando el servicio
            var candidatos = await _asignacionService.GetCandidatosDisponiblesAsync(partidoId);
            var puestos = await _asignacionService.GetPuestosDisponiblesAsync(partidoId);

            vm.CandidatosDisponibles = candidatos.Select(c => new SelectListItem 
            { 
                Value = c.Id.ToString(), 
                Text = $"{c.NombreCandidato} {c.ApellidoCandidato}" 
            });

            vm.PuestosDisponibles = puestos.Select(p => new SelectListItem 
            { 
                Value = p.Id.ToString(), 
                Text = p.Nombre 
            });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(AgregarAsignacionesViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                var partidoId = int.Parse(User.FindFirst("PartidoId")?.Value ?? "0");

                await _asignacionService.AgregarAsync(vm.CandidatoId, vm.PuestoElectivoId, partidoId);

                TempData["Exito"] = "Asignación creada con éxito.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }
    }
}