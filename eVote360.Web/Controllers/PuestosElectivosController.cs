using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.PuestosElectivos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PuestosElectivosController : Controller
    {
        private readonly IPuestoElectivoService _puestoService;
        private readonly IEleccionValidationService _validationService;

        public PuestosElectivosController(IPuestoElectivoService puestoService, IEleccionValidationService validationService)
        {
            _puestoService = puestoService;
            _validationService = validationService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _puestoService.GetAllAsync();
            ViewBag.IsEleccionActiva = await _validationService.HasEleccionActivaAsync();
            return View(dtos);
        }

        public async Task<IActionResult> Create()
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            return View(new SavePuestoElectivoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePuestoElectivoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SavePuestoElectivoDto { Nombre = vm.Nombre, Descripcion = vm.Descripcion };
                await _puestoService.AddAsync(dto);
                TempData["SuccessMessage"] = "Puesto electivo creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _puestoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            var vm = new SavePuestoElectivoViewModel { Id = dto.Id, Nombre = dto.Nombre, Descripcion = dto.Descripcion };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePuestoElectivoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SavePuestoElectivoDto { Id = vm.Id, Nombre = vm.Nombre, Descripcion = vm.Descripcion };
                await _puestoService.UpdateAsync(dto);
                TempData["SuccessMessage"] = "Puesto electivo actualizado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeState(int id, bool state)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar puestos electivos mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _puestoService.ChangeStateAsync(id, state);
                TempData["SuccessMessage"] = $"El puesto electivo ha sido {(state ? "activado" : "inactivado")} con éxito.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}