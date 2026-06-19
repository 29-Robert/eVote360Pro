using eVote360.Core.Application.DTOs.Asignaciones; // Deberías tener un DTO específico para esto
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Asignaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var claimPartido = User.FindFirst("PartidoId");
            if (claimPartido == null) return RedirectToAction("Index", "Home");

            var partidoId = int.Parse(claimPartido.Value);
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

            var claimPartido = User.FindFirst("PartidoId");
            if (claimPartido == null) return RedirectToAction("Index", "Home");
            var partidoId = int.Parse(User.FindFirst("PartidoId")?.Value ?? "0");
            var vm = new AgregarAsignacionesViewModel();

            var partidoId = int.Parse(claimPartido.Value);

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
                var claimPartido = User.FindFirst("PartidoId");
                if (claimPartido == null) return RedirectToAction("Index", "Home");

                var partidoId = int.Parse(claimPartido.Value);

                // Aquí llamamos al servicio directamente sin usar el DTO fantasma
                var partidoId = int.Parse(User.FindFirst("PartidoId")?.Value ?? "0");

                await _asignacionService.AgregarAsync(vm.CandidatoId, vm.PuestoElectivoId, partidoId);

                TempData["Exito"] = "Asignación agregada correctamente.";

                TempData["Exito"] = "Asignación creada con éxito.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var asignacion = await _asignacionRepo.GetByIdAsync(id);
            return View(asignacion);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _asignacionService.EliminarAsync(id);
                TempData["Exito"] = "Asignación eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
            return RedirectToAction("Index");
        }
    }
}