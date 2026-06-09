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
        public async Task<IActionResult> Create(SavePuestoElectivoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new SavePuestoElectivoDto
            {
                Nombre = vm.Nombre,
                Descripcion = vm.Descripcion
            };

            await _puestoService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _puestoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            var vm = new SavePuestoElectivoViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavePuestoElectivoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new SavePuestoElectivoDto
            {
                Id = vm.Id,
                Nombre = vm.Nombre,
                Descripcion = vm.Descripcion
            };

            await _puestoService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeState(int id, bool state)
        {
            await _puestoService.ChangeStateAsync(id, state);
            return RedirectToAction(nameof(Index));
        }
    }
}