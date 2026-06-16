using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Candidatos;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly ICandidatoService _candidatoService;
        private readonly IEleccionRepository _eleccionRepo;

        public CandidatosController(ICandidatoService candidatoService,
                                     IEleccionRepository eleccionRepo)
        {
            _candidatoService = candidatoService;
            _eleccionRepo = eleccionRepo;
        }

        public async Task<IActionResult> Index()
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var candidatos = await _candidatoService.GetByPartidoAsync(partidoId);
            return View(candidatos);
        }

        public async Task<IActionResult> Crear()
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
            {
                TempData["Error"] = "No se pueden modificar candidatos mientras exista una elección activa.";
                return RedirectToAction("Index");
            }
            return View(new CrearCandidatosViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearCandidatosViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var partidoId = int.Parse(User.FindFirst("PartidoId").Value);

                // Guardar foto
                var fotoPath = await GuardarFotoAsync(vm.Foto);

                var dto = new CrearCandidatoDto
                {
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Foto = fotoPath,
                    EsActivo = vm.EsActivo,
                    PartidoPoliticoId = partidoId
                };

                await _candidatoService.CreateAsync(dto);
                TempData["Exito"] = "Candidato creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
            {
                TempData["Error"] = "No se pueden modificar candidatos mientras exista una elección activa.";
                return RedirectToAction("Index");
            }

            var candidato = await _candidatoService.GetByIdAsync(id);
            var vm = new EditarCandidatosViewModel
            {
                Id = candidato.Id,
                Nombre = candidato.Nombre,
                Apellido = candidato.Apellido,
                EsActivo = candidato.EsActivo
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(EditarCandidatosViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var partidoId = int.Parse(User.FindFirst("PartidoId").Value);

                string? fotoPath = null;
                if (vm.Foto != null)
                    fotoPath = await GuardarFotoAsync(vm.Foto);

                var dto = new CandidatoUpdateDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Foto = fotoPath,
                    EsActivo = vm.EsActivo,
                    PartidoPoliticoId = partidoId
                };

                await _candidatoService.UpdateAsync(dto);
                TempData["Exito"] = "Candidato actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Activar(int id)
        {
            var candidato = await _candidatoService.GetByIdAsync(id);
            return View(candidato);
        }

        [HttpPost]
        public async Task<IActionResult> ActivarConfirmado(int id)
        {
            try
            {
                await _candidatoService.ActivarAsync(id);
                TempData["Exito"] = "Candidato activado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Desactivar(int id)
        {
            var candidato = await _candidatoService.GetByIdAsync(id);
            return View(candidato);
        }

        [HttpPost]
        public async Task<IActionResult> DesactivarConfirmado(int id)
        {
            try
            {
                await _candidatoService.DesactivarAsync(id);
                TempData["Exito"] = "Candidato desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        private async Task<string> GuardarFotoAsync(IFormFile foto)
        {
            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/candidatos");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await foto.CopyToAsync(stream);

            return $"/img/candidatos/{nombreArchivo}";
        }
    }

}
