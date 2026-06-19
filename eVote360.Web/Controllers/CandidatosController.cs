using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360.WebApp.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _candidatoService;
        private readonly IPartidoPoliticoService _partidoService;
        private readonly IPuestoElectivoService _puestoService;

        public CandidatosController(ICandidatoService candidatoService,
                                     IPartidoPoliticoService partidoService,
                                     IPuestoElectivoService puestoService)
        {
            _candidatoService = candidatoService;
            _partidoService = partidoService;
            _puestoService = puestoService;
        }

        public async Task<IActionResult> Index()
        {
            var candidatos = await _candidatoService.GetByPartidoAsync(0); 
            return View(candidatos);
        }

        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            ViewBag.Partidos = new SelectList(await _partidoService.GetAllAsync(), "Id", "Nombre");
            ViewBag.Puestos = new SelectList(await _puestoService.GetAllAsync(), "Id", "Nombre");

            if (id == 0) return View(new SaveCandidatoDto());

            var dto = await _candidatoService.GetByIdAsync(id);
            return View(new SaveCandidatoDto
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(SaveCandidatoDto dto, IFormFile file)
        {
            if (!ModelState.IsValid) return View(dto);

            if (file != null)
                dto.Foto = UploadHelper.UploadImage(file, "images/candidatos");

            try
            {
                if (dto.Id == 0) await _candidatoService.AddAsync(dto);
                else await _candidatoService.UpdateAsync(dto);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeState(int id, bool estado)
        {
            try
            {
                await _candidatoService.ChangeStateAsync(id, estado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}