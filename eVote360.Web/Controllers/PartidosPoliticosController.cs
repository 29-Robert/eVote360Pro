using eVote360.Core.Application.DTOs.PartidosPoliticos;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    public class PartidosPoliticosController : Controller
    {
        private readonly IPartidoPoliticoService _partidoService;

        public PartidosPoliticosController(IPartidoPoliticoService partidoService)
        {
            _partidoService = partidoService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _partidoService.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new SavePartidoPoliticoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePartidoPoliticoDto dto, IFormFile file)
        {
            if (!ModelState.IsValid) return View(dto);

            if (file != null)
            {
                dto.LogoUrl = UploadHelper.UploadImage(file, "images/partidos");
            }

            try
            {
                await _partidoService.AddAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _partidoService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePartidoPoliticoDto dto, IFormFile file) // AÑADIDO: Parámetro file
        {
            if (!ModelState.IsValid) return View(dto);

            if (file != null)
            {
                dto.LogoUrl = UploadHelper.UploadImage(file, "images/partidos");
            }

            try
            {
                await _partidoService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeState(int id)
        {
            try
            {
                await _partidoService.ChangeStateAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}