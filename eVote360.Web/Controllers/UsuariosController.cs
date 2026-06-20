using eVote360.Core.Application.DTOs.Usuarios;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
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
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
            }

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
                TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear el usuario: {ex.Message}";
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

            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUsuarioViewModel vm)
        {
            if (await _validationService.HasEleccionActivaAsync())
            {
                TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista una elección activa.";
                return RedirectToAction(nameof(Index));
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

                await _usuarioService.UpdateAsync(dto);
                TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar el usuario: {ex.Message}";
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeState(int id, bool state)
        {
            try
            {
                if (await _validationService.HasEleccionActivaAsync())
                {
                    TempData["ErrorMessage"] = "No se pueden modificar usuarios mientras exista una elección activa.";
                    return RedirectToAction(nameof(Index));
                }

                await _usuarioService.ChangeStateAsync(id, state);
                TempData["SuccessMessage"] = $"Usuario {(state ? "activado" : "desactivado")} exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar el estado del usuario: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}