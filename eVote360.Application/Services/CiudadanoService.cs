using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class CiudadanoService : ICiudadanoService
    {
        private readonly IGenericRepository<Ciudadano> _repository;

        public CiudadanoService(IGenericRepository<Ciudadano> repository)
        {
            _repository = repository;
        }

        public async Task<SaveCiudadanoDto> AddAsync(SaveCiudadanoDto dto)
        {
            var ciudadanos = await _repository.GetAllAsync();
            if (ciudadanos.Any(c => c.DocumentoIdentidad == dto.DocumentoIdentidad || c.CorreoElectronico == dto.CorreoElectronico))
            {
                throw new InvalidOperationException("El documento de identidad o correo ya existe.");
            }

            var entity = new Ciudadano
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                CorreoElectronico = dto.CorreoElectronico,
                DocumentoIdentidad = dto.DocumentoIdentidad,
                Estado = true
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            return dto;
        }

        public async Task UpdateAsync(SaveCiudadanoDto dto)
        {
            var ciudadanos = await _repository.GetAllAsync();
            if (ciudadanos.Any(c => (c.DocumentoIdentidad == dto.DocumentoIdentidad || c.CorreoElectronico == dto.CorreoElectronico) && c.Id != dto.Id))
            {
                throw new InvalidOperationException("El documento de identidad o correo ya existe.");
            }

            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Nombre = dto.Nombre;
                entity.Apellido = dto.Apellido;
                entity.CorreoElectronico = dto.CorreoElectronico;
                entity.DocumentoIdentidad = dto.DocumentoIdentidad;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task<List<CiudadanoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(e => new CiudadanoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                CorreoElectronico = e.CorreoElectronico,
                DocumentoIdentidad = e.DocumentoIdentidad,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<SaveCiudadanoDto?> GetByIdSaveDtoAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SaveCiudadanoDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                CorreoElectronico = entity.CorreoElectronico,
                DocumentoIdentidad = entity.DocumentoIdentidad
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
    }
}