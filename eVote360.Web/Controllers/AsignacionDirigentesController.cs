using eVote360.Core.Application.DTOs.Asignaciones;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.AsignacionDirigentes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AsignacionDirigentesController : Controller
    {
        private readonly IAsignacionDirigenteService _asignacionService;
        private readonly IUsuarioService _usuarioService;

        private readonly IPartidoPoliticoService _partidoService;
        private readonly IEleccionValidationService _validationService;

        public AsignacionDirigentesController(
            IAsignacionDirigenteService asignacionService,
            IUsuarioService usuarioService,
            IPartidoPoliticoService partidoService,
            IEleccionValidationService validationService)
        {
            _asignacionService = asignacionService;
            _usuarioService = usuarioService;
            _partidoService = partidoService;
            _validationService = validationService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.IsEleccionActiva = await _validationService.HasEleccionActivaAsync();
            var asignaciones = await _asignacionService.GetAllAsync();
            return View(asignaciones);
        }

        public async Task<IActionResult> Create()
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            await CargarViewBags();
            return View(new SaveAsignacionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveAsignacionViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CargarViewBags();
                return View(vm);
            }

            try
            {
                var dto = new SaveAsignacionDirigenteDto
                {
                    UsuarioId = vm.UsuarioId,
                    PartidoPoliticoId = vm.PartidoPoliticoId
                };

                await _asignacionService.AddAsync(dto);
                TempData["SuccessMessage"] = "Dirigente asignado al partido político exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarViewBags();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar asignaciones mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _asignacionService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Asignación eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarViewBags()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var partidos = await _partidoService.GetAllAsync();
            var asignaciones = await _asignacionService.GetAllAsync();

            var usuariosOcupadosIds = asignaciones.Select(a => a.UsuarioId).ToList();
            var partidosOcupadosIds = asignaciones.Select(a => a.PartidoPoliticoId).ToList();

            var dirigentesDisponibles = usuarios
                .Where(u => u.Rol == "Dirigente político" && u.Estado && !usuariosOcupadosIds.Contains(u.Id))
                .Select(u => new { u.Id, NombreMostrado = $"{u.Nombre} {u.Apellido} - {u.NombreUsuario}" })
                .ToList();

            var partidosDisponibles = partidos
                .Where(p => p.Estado && !partidosOcupadosIds.Contains(p.Id))
                .Select(p => new { p.Id, NombreMostrado = $"{p.Nombre} ({p.Siglas})" })
                .ToList();

            ViewBag.Usuarios = new SelectList(dirigentesDisponibles, "Id", "NombreMostrado");
            ViewBag.Partidos = new SelectList(partidosDisponibles, "Id", "NombreMostrado");
        }
    }
}