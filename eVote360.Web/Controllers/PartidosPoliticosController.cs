using eVote360.Core.Application.DTOs.PartidosPoliticos;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.PartidosPoliticos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    public class PartidosPoliticosController : Controller
    {
        private readonly IPartidoPoliticoService _partidoService;
        private readonly IEleccionValidationService _validationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PartidosPoliticosController(
            IPartidoPoliticoService partidoService,
            IEleccionValidationService validationService,
            IWebHostEnvironment webHostEnvironment)
        {
            _partidoService = partidoService;
            _validationService = validationService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _partidoService.GetAllAsync();
            ViewBag.IsEleccionActiva = await _validationService.HasEleccionActivaAsync();
            return View(dtos);
        }

        public async Task<IActionResult> Create()
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "Acción bloqueada: Hay una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            return View(new SavePartidoPoliticoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePartidoPoliticoViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync()) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid) return View(vm);

            try
            {
                string logoRuta = string.Empty;

                if (vm.LogoFile != null)
                {
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/partidos");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string fileName = Guid.NewGuid().ToString() + "_" + vm.LogoFile.FileName;
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.LogoFile.CopyToAsync(fileStream);
                    }
                    logoRuta = $"/images/partidos/{fileName}";
                }

                var dto = new SavePartidoPoliticoDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Siglas = vm.Siglas,
                    LogoUrl = logoRuta
                };

                await _partidoService.AddAsync(dto);
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
            if (await _validationService.HasEleccionActivaAsync()) return RedirectToAction(nameof(Index));

            await _partidoService.ChangeStateAsync(id, state);
            return RedirectToAction(nameof(Index));
        }
    }
}