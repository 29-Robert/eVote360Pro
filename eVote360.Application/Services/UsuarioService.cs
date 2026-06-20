using eVote360.Core.Application.DTOs.Usuarios;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repository;

        public UsuarioService(IGenericRepository<Usuario> repository)
        {
            _repository = repository;
        }

        public async Task<SaveUsuarioDto> AddAsync(SaveUsuarioDto dto)
        {

            if (dto.Rol != "Administrador" && dto.Rol != "Dirigente político")
            {
                throw new InvalidOperationException("El rol seleccionado es inválido. Solo se permiten 'Administrador' o 'Dirigente político'.");
            }

            var usuarios = await _repository.GetAllAsync();
            if (usuarios.Any(u => u.NombreUsuario.Trim().ToLower() == dto.NombreUsuario.Trim().ToLower() || u.CorreoElectronico.Trim().ToLower() == dto.CorreoElectronico.Trim().ToLower()))
            {
                throw new InvalidOperationException("El nombre de usuario o correo electrónico ya existe.");
            }

            var entity = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                CorreoElectronico = dto.CorreoElectronico.Trim().ToLower(),
                NombreUsuario = dto.NombreUsuario.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Rol,
                Estado = true
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            return dto;
        }

        public async Task UpdateAsync(SaveUsuarioDto dto)
        {

            if (dto.Rol != "Administrador" && dto.Rol != "Dirigente político")
            {
                throw new InvalidOperationException("El rol seleccionado es inválido. Solo se permiten 'Administrador' o 'Dirigente político'.");
            }

            var usuarios = await _repository.GetAllAsync();
            if (usuarios.Any(u => (u.NombreUsuario.Trim().ToLower() == dto.NombreUsuario.Trim().ToLower() || u.CorreoElectronico.Trim().ToLower() == dto.CorreoElectronico.Trim().ToLower()) && u.Id != dto.Id))
            {
                throw new InvalidOperationException("El nombre de usuario o correo electrónico ya existe.");
            }

            var entity = await _repository.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"El usuario con ID {dto.Id} no existe.");


            if (entity.Rol == "Administrador" && entity.Estado && dto.Rol != "Administrador")
            {
                var activeAdmins = usuarios.Count(u => u.Rol == "Administrador" && u.Estado && u.Id != dto.Id);
                if (activeAdmins == 0)
                {
                    throw new InvalidOperationException("No se puede cambiar el rol al único administrador activo del sistema.");
                }
            }

            entity.Nombre = dto.Nombre.Trim();
            entity.Apellido = dto.Apellido.Trim();
            entity.CorreoElectronico = dto.CorreoElectronico.Trim().ToLower();
            entity.NombreUsuario = dto.NombreUsuario.Trim();
            entity.Rol = dto.Rol;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _repository.UpdateAsync(entity);
        }

        public async Task<List<UsuarioDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(e => new UsuarioDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                CorreoElectronico = e.CorreoElectronico,
                NombreUsuario = e.NombreUsuario,
                Rol = e.Rol,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<SaveUsuarioDto?> GetByIdSaveDtoAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SaveUsuarioDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                CorreoElectronico = entity.CorreoElectronico,
                NombreUsuario = entity.NombreUsuario,
                Rol = entity.Rol
            };
        }

        public async Task ChangeStateAsync(int id, bool state)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"El usuario con ID {id} no existe.");


            if (!state && entity.Rol == "Administrador" && entity.Estado)
            {
                var usuarios = await _repository.GetAllAsync();
                var activeAdmins = usuarios.Count(u => u.Rol == "Administrador" && u.Estado && u.Id != id);
                if (activeAdmins == 0)
                {
                    throw new InvalidOperationException("No se puede desactivar al único administrador activo del sistema.");
                }
            }

            entity.Estado = state;
            await _repository.UpdateAsync(entity);
        }

        public async Task<UsuarioDto?> LoginAsync(LoginDto dto)
        {
            var usuarios = await _repository.GetAllAsync();

            var usuarioEntity = usuarios.FirstOrDefault(u =>
                u.NombreUsuario.Equals(dto.NombreUsuario, StringComparison.OrdinalIgnoreCase));

            if (usuarioEntity == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuarioEntity.PasswordHash))
            {
                return null;
            }

            return new UsuarioDto
            {
                Id = usuarioEntity.Id,
                Nombre = usuarioEntity.Nombre,
                Apellido = usuarioEntity.Apellido,
                CorreoElectronico = usuarioEntity.CorreoElectronico,
                NombreUsuario = usuarioEntity.NombreUsuario,
                Rol = usuarioEntity.Rol,
                Estado = usuarioEntity.Estado
            };
        }
    }
}