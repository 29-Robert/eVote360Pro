using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Ciudadanos;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    public class CiudadanosController : Controller
    {
        private readonly ICiudadanoService _ciudadanoService;
        private readonly IEleccionValidationService _validationService;

        public CiudadanosController(ICiudadanoService ciudadanoService, IEleccionValidationService validationService)
        {
            _ciudadanoService = ciudadanoService;
            _validationService = validationService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _ciudadanoService.GetAllAsync();
            ViewBag.IsEleccionActiva = await _validationService.HasEleccionActivaAsync();
            return View(dtos);
        }

        public async Task<IActionResult> Create()
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "Acción bloqueada: Existe una elección activa en el sistema.";
                return RedirectToAction(nameof(Index));
            }
            return View(new SaveCiudadanoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCiudadanoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden agregar ciudadanos mientras una elección esté activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SaveCiudadanoDto
                {
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    CorreoElectronico = vm.CorreoElectronico,
                    DocumentoIdentidad = vm.DocumentoIdentidad.Replace("-", "")
                };

                await _ciudadanoService.AddAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "Acción bloqueada: Existe una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _ciudadanoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            var vm = new SaveCiudadanoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                CorreoElectronico = dto.CorreoElectronico,
                DocumentoIdentidad = dto.DocumentoIdentidad
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCiudadanoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar datos mientras una elección esté activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SaveCiudadanoDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    CorreoElectronico = vm.CorreoElectronico,
                    DocumentoIdentidad = vm.DocumentoIdentidad.Replace("-", "")
                };

                await _ciudadanoService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeState(int id, bool state)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede cambiar el estado de un ciudadano durante una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            await _ciudadanoService.ChangeStateAsync(id, state);
            return RedirectToAction(nameof(Index));
        }
    }
}