using eVote360.Core.Application.DTOs.Usuarios;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Usuarios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEleccionValidationService _validationService;

        public UsuariosController(IUsuarioService usuarioService, IEleccionValidationService validationService)
        {
            _usuarioService = usuarioService;
            _validationService = validationService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            ViewBag.IsEleccionActiva = await _validationService.HasEleccionActivaAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }
            return View(new SaveUsuarioViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveUsuarioViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync()) return RedirectToAction(nameof(Index));

            if (string.IsNullOrEmpty(vm.Contrasena))
            {
                ModelState.AddModelError("Contrasena", "La contraseña es obligatoria al crear un usuario.");
            }

            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SaveUsuarioDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    CorreoElectronico = vm.Correo,
                    NombreUsuario = vm.NombreUsuario,
                    Password = vm.Contrasena,
                    ConfirmarPassword = vm.ConfirmarContrasena,
                    Rol = vm.Rol
                };

                await _usuarioService.AddAsync(dto);
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
                TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

            // Corrección: Usando el método correcto de tu interfaz
            var dto = await _usuarioService.GetByIdSaveDtoAsync(id);
            if (dto == null) return NotFound();

            var vm = new SaveUsuarioViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Correo = dto.CorreoElectronico,
                NombreUsuario = dto.NombreUsuario,
                Rol = dto.Rol
                // La propiedad Estado no se asigna porque no está en el DTO
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUsuarioViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync()) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new SaveUsuarioDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    CorreoElectronico = vm.Correo,
                    NombreUsuario = vm.NombreUsuario,
                    Password = vm.Contrasena,
                    ConfirmarPassword = vm.ConfirmarContrasena,
                    Rol = vm.Rol
                };

                await _usuarioService.UpdateAsync(dto);
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

            await _usuarioService.ChangeStateAsync(id, state);
            return RedirectToAction(nameof(Index));
        }
    }
}