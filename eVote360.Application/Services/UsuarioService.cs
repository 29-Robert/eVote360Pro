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
            var usuarios = await _repository.GetAllAsync();
            if (usuarios.Any(u => u.NombreUsuario == dto.NombreUsuario || u.CorreoElectronico == dto.CorreoElectronico))
            {
                throw new InvalidOperationException("El nombre de usuario o correo ya existe.");
            }

            var entity = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                CorreoElectronico = dto.CorreoElectronico,
                NombreUsuario = dto.NombreUsuario,
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
            var usuarios = await _repository.GetAllAsync();
            if (usuarios.Any(u => (u.NombreUsuario == dto.NombreUsuario || u.CorreoElectronico == dto.CorreoElectronico) && u.Id != dto.Id))
            {
                throw new InvalidOperationException("El nombre de usuario o correo ya existe.");
            }

            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Nombre = dto.Nombre;
                entity.Apellido = dto.Apellido;
                entity.CorreoElectronico = dto.CorreoElectronico;
                entity.NombreUsuario = dto.NombreUsuario;
                entity.Rol = dto.Rol;

                if (!string.IsNullOrEmpty(dto.Password))
                {
                    entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                await _repository.UpdateAsync(entity);
            }
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
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.Estado = state;
                await _repository.UpdateAsync(entity);
            }
        }

        // --- MÉTODO LOGIN ASYNC IMPLEMENTADO ---
        public async Task<UsuarioDto?> LoginAsync(LoginDto dto)
        {
            var usuarios = await _repository.GetAllAsync();

            // Buscamos primero al usuario por su nombre de usuario único
            var usuarioEntity = usuarios.FirstOrDefault(u =>
                u.NombreUsuario.Equals(dto.NombreUsuario, StringComparison.OrdinalIgnoreCase));

            // Si no existe el usuario o la contraseña encriptada no coincide, retornamos null (Credenciales inválidas)
            if (usuarioEntity == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuarioEntity.PasswordHash))
            {
                return null;
            }

            // Si todo está correcto, mapeamos y retornamos el UsuarioDto
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