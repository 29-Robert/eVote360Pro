using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eVote360.Core.Application.DTOs.PartidosPoliticos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;

namespace eVote360.Core.Application.Services
{
    public class PartidoPoliticoService : IPartidoPoliticoService
    {
        private readonly IGenericRepository<PartidoPolitico> _repository;
        private readonly IGenericRepository<Eleccion> _eleccionRepository;
        private readonly IVotoRepository _votoRepository;
        private readonly IGenericRepository<Candidato> _candidatoRepository;

        public PartidoPoliticoService(
            IGenericRepository<PartidoPolitico> repository,
            IGenericRepository<Eleccion> eleccionRepository,
            IVotoRepository votoRepository,
            IGenericRepository<Candidato> candidatoRepository)
        {
            _repository = repository;
            _eleccionRepository = eleccionRepository;
            _votoRepository = votoRepository;
            _candidatoRepository = candidatoRepository;
        }

        private async Task EnsureNoEleccionActivaAsync(string mensaje)
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.Estado == EstadoEleccion.Activa))
            {
                throw new InvalidOperationException(mensaje);
            }
        }

        public async Task<bool> PuedeModificarPartidoAsync(int partidoId)
        {

            var candidatos = await _candidatoRepository.GetAllAsync();
            var candidatoIds = candidatos.Where(c => c.PartidoPoliticoId == partidoId).Select(c => c.Id).ToList();

            var votos = await _votoRepository.GetAllAsync();
            return !votos.Any(v => v.CandidatoId.HasValue && candidatoIds.Contains(v.CandidatoId.Value));
        }

        public async Task<SavePartidoPoliticoDto> AddAsync(SavePartidoPoliticoDto dto)
        {
            await EnsureNoEleccionActivaAsync("No se puede crear un partido político mientras exista una elección activa.");

            var partidos = await _repository.GetAllAsync();
            if (partidos.Any(p => p.Siglas.Trim().ToUpper() == dto.Siglas.Trim().ToUpper()))
            {
                throw new InvalidOperationException("Las siglas ingresadas ya existen.");
            }

            var entity = new PartidoPolitico
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                Siglas = dto.Siglas.Trim().ToUpper(),
                LogoUrl = dto.LogoUrl,
                Estado = true
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            dto.PuedeEditarCamposCriticos = true;
            return dto;
        }

        public async Task UpdateAsync(SavePartidoPoliticoDto dto)
        {
            await EnsureNoEleccionActivaAsync("No se puede editar un partido político mientras exista una elección activa.");

            var entity = await _repository.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"El partido político con ID {dto.Id} no existe.");

            var partidos = await _repository.GetAllAsync();
            if (partidos.Any(p => p.Siglas.Trim().ToUpper() == dto.Siglas.Trim().ToUpper() && p.Id != dto.Id))
            {
                throw new InvalidOperationException("Las siglas ingresadas ya existen.");
            }


            if (entity.Nombre.Trim() != dto.Nombre.Trim() || 
                entity.Siglas.Trim().ToUpper() != dto.Siglas.Trim().ToUpper() || 
                entity.LogoUrl != dto.LogoUrl)
            {
                if (!await PuedeModificarPartidoAsync(dto.Id))
                {
                    throw new InvalidOperationException("No se pueden modificar el nombre, siglas o logo del partido porque ya participó en una elección.");
                }
            }

            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion?.Trim();
            entity.Siglas = dto.Siglas.Trim().ToUpper();
            entity.LogoUrl = dto.LogoUrl;

            await _repository.UpdateAsync(entity);
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
                LogoUrl = entity.LogoUrl,
                PuedeEditarCamposCriticos = await PuedeModificarPartidoAsync(id)
            };
        }

        public async Task ChangeStateAsync(int id, bool state)
        {
            await EnsureNoEleccionActivaAsync("No se puede cambiar el estado de un partido político mientras exista una elección activa.");

            var partido = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"El partido político con ID {id} no existe.");

            partido.Estado = state;
            await _repository.UpdateAsync(partido);
        }
    }
}