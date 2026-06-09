using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class PuestoElectivoService : IPuestoElectivoService
    {
        private readonly IGenericRepository<PuestoElectivo> _repository;

        public PuestoElectivoService(IGenericRepository<PuestoElectivo> repository)
        {
            _repository = repository;
        }

        public async Task<SavePuestoElectivoDto> AddAsync(SavePuestoElectivoDto dto)
        {
            var entity = new PuestoElectivo
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Estado = true
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            return dto;
        }

        public async Task UpdateAsync(SavePuestoElectivoDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Nombre = dto.Nombre;
                entity.Descripcion = dto.Descripcion;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task<List<PuestoElectivoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(e => new PuestoElectivoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<SavePuestoElectivoDto?> GetByIdSaveDtoAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SavePuestoElectivoDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion
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