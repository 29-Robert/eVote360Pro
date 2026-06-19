using eVote360.Core.Application.DTOs.PartidosPoliticos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class PartidoPoliticoService : IPartidoPoliticoService
    {
        private readonly IGenericRepository<PartidoPolitico> _repository;
        private readonly IGenericRepository<Eleccion> _eleccionRepository;

        public PartidoPoliticoService(IGenericRepository<PartidoPolitico> repository, IGenericRepository<Eleccion> eleccionRepository)
        {
            _repository = repository;
            _eleccionRepository = eleccionRepository;
        }

        public async Task<SavePartidoPoliticoDto> AddAsync(SavePartidoPoliticoDto dto)
        {
            var partidos = await _repository.GetAllAsync();
            if (partidos.Any(p => p.Siglas.ToUpper() == dto.Siglas.ToUpper()))
            {
                throw new InvalidOperationException("Las siglas ingresadas ya existen.");
            }

            var entity = new PartidoPolitico
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Siglas = dto.Siglas.ToUpper(),
                LogoUrl = dto.LogoUrl,
                Estado = true
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            return dto;
        }

        public async Task UpdateAsync(SavePartidoPoliticoDto dto)
        {
            var partidos = await _repository.GetAllAsync();
            if (partidos.Any(p => p.Siglas.ToUpper() == dto.Siglas.ToUpper() && p.Id != dto.Id))
            {
                throw new InvalidOperationException("Las siglas ingresadas ya existen.");
            }

            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Nombre = dto.Nombre;
                entity.Descripcion = dto.Descripcion;
                entity.Siglas = dto.Siglas.ToUpper();
                entity.LogoUrl = dto.LogoUrl;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task<List<PartidoPoliticoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(e => new PartidoPoliticoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Siglas = e.Siglas,
                LogoUrl = e.LogoUrl,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<SavePartidoPoliticoDto?> GetByIdSaveDtoAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SavePartidoPoliticoDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Siglas = entity.Siglas,
                LogoUrl = entity.LogoUrl
            };
        }

        public async Task ChangeStateAsync(int id, bool state)
        {
            var partido = await _repository.GetByIdAsync(id);
            if (partido == null) throw new InvalidOperationException("Partido no encontrado");

            partido.Estado = state;
            await _repository.UpdateAsync(partido);
        }
    }
}