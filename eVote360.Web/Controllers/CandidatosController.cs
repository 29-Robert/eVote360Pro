using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Dirigente político")]
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

        private int GetPartidoId()
        {
            var claim = User.FindFirst("PartidoId")?.Value;
            return claim != null ? int.Parse(claim) : 0;
        }

        public async Task<IActionResult> Index()
        {
            int partidoId = GetPartidoId();
            var candidatos = await _candidatoService.GetByPartidoAsync(partidoId); 
            return View(candidatos);
        }

        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            int partidoId = GetPartidoId();
            ViewBag.Partidos = new SelectList(await _partidoService.GetAllAsync(), "Id", "Nombre", partidoId);
            ViewBag.Puestos = new SelectList(await _puestoService.GetAllAsync(), "Id", "Nombre");

            if (id == 0)
            {
                var newDto = new SaveCandidatoDto { PartidoPoliticoId = partidoId };
                return View(newDto);
            }

            var dto = await _candidatoService.GetByIdAsync(id);
            return View(new SaveCandidatoDto
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DocumentoIdentidad = dto.DocumentoIdentidad,
                CorreoElectronico = dto.CorreoElectronico,
                PartidoPoliticoId = dto.PartidoPoliticoId,
                PuestoElectivoId = dto.PuestoElectivoId,
                Foto = dto.Foto
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(SaveCandidatoDto dto, IFormFile file)
        {
            int partidoId = GetPartidoId();
            dto.PartidoPoliticoId = partidoId;


            ModelState.Remove(nameof(dto.PartidoPoliticoId));
            ModelState.Remove(nameof(dto.Foto));

            if (!ModelState.IsValid)
            {
                ViewBag.Partidos = new SelectList(await _partidoService.GetAllAsync(), "Id", "Nombre", partidoId);
                ViewBag.Puestos = new SelectList(await _puestoService.GetAllAsync(), "Id", "Nombre");
                return View(dto);
            }

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
                ViewBag.Partidos = new SelectList(await _partidoService.GetAllAsync(), "Id", "Nombre", partidoId);
                ViewBag.Puestos = new SelectList(await _puestoService.GetAllAsync(), "Id", "Nombre");
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