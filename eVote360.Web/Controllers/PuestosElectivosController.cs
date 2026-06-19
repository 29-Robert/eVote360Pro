using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.PuestosElectivos;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    public class PuestosElectivosController : Controller
    {
        private readonly IPuestoElectivoService _puestoService;

        public PuestosElectivosController(IPuestoElectivoService puestoService)
        {
            _puestoService = puestoService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _puestoService.GetAllAsync();
            var viewModels = dtos.Select(d => new PuestoElectivoViewModel
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                Estado = d.Estado
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Create()
        {
            return View(new SavePuestoElectivoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePuestoElectivoViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SavePuestoElectivoDto { Nombre = vm.Nombre, Descripcion = vm.Descripcion };
                await _puestoService.AddAsync(dto);
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
            var dto = await _puestoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            return View(new SavePuestoElectivoViewModel { Id = dto.Id, Nombre = dto.Nombre, Descripcion = dto.Descripcion });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePuestoElectivoViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SavePuestoElectivoDto { Id = vm.Id, Nombre = vm.Nombre, Descripcion = vm.Descripcion };
                await _puestoService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeState(int id)
        {
            try
            {
                await _puestoService.ChangeStateAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}