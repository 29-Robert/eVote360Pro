using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Ciudadanos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
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
            var viewModels = dtos.Select(MapToViewModel).ToList();

            ViewBag.IsEleccionActiva = await _validationService.HasEleccionActivaAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SaveCiudadanoViewModel { Estado = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveCiudadanoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede crear un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _ciudadanoService.AddAsync(MapToDto(vm));
                TempData["SuccessMessage"] = "Ciudadano registrado correctamente.";
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
                TempData["ErrorMessage"] = "No se puede editar un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await _ciudadanoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            return View(MapToSaveViewModel(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveCiudadanoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se puede editar un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _ciudadanoService.UpdateAsync(MapToDto(vm));
                TempData["SuccessMessage"] = "Ciudadano actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeState(int id, bool state)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = state
                    ? "No se puede activar un ciudadano mientras exista una elección activa."
                    : "No se puede desactivar un ciudadano mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _ciudadanoService.ChangeStateAsync(id, state);
                TempData["SuccessMessage"] = state
                    ? "Ciudadano activado correctamente."
                    : "Ciudadano desactivado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static CiudadanoViewModel MapToViewModel(CiudadanoDto dto) => new()
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            CorreoElectronico = dto.CorreoElectronico,
            DocumentoIdentidad = dto.DocumentoIdentidad,
            Estado = dto.Estado
        };

        private static SaveCiudadanoViewModel MapToSaveViewModel(SaveCiudadanoDto dto) => new()
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            CorreoElectronico = dto.CorreoElectronico,
            DocumentoIdentidad = dto.DocumentoIdentidad,
            Estado = dto.Estado,
            PuedeEditarDocumentoIdentidad = dto.PuedeEditarDocumentoIdentidad
        };

        private static SaveCiudadanoDto MapToDto(SaveCiudadanoViewModel vm) => new()
        {
            Id = vm.Id,
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            CorreoElectronico = vm.CorreoElectronico,
            DocumentoIdentidad = vm.DocumentoIdentidad,
            Estado = vm.Estado,
            PuedeEditarDocumentoIdentidad = vm.PuedeEditarDocumentoIdentidad
        };
    }
}