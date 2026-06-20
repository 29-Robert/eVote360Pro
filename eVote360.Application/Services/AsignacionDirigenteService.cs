using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eVote360.Core.Application.DTOs.Asignaciones;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;

namespace eVote360.Core.Application.Services
{
    public class AsignacionDirigenteService : IAsignacionDirigenteService
    {
        private readonly IGenericRepository<AsignacionDirigente> _repository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IGenericRepository<PartidoPolitico> _partidoRepository;
        private readonly IGenericRepository<Eleccion> _eleccionRepository;

        public AsignacionDirigenteService(
            IGenericRepository<AsignacionDirigente> repository,
            IGenericRepository<Usuario> usuarioRepository,
            IGenericRepository<PartidoPolitico> partidoRepository,
            IGenericRepository<Eleccion> eleccionRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _partidoRepository = partidoRepository;
            _eleccionRepository = eleccionRepository;
        }

        private async Task EnsureNoEleccionActivaAsync(string mensaje)
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.Estado == EstadoEleccion.Activa))
            {
                throw new InvalidOperationException(mensaje);
            }
        }

        public async Task<SaveAsignacionDirigenteDto> AddAsync(SaveAsignacionDirigenteDto dto)
        {
            await EnsureNoEleccionActivaAsync("No se pueden crear asignaciones mientras exista una elección activa.");

            var user = await _usuarioRepository.GetByIdAsync(dto.UsuarioId)
                ?? throw new KeyNotFoundException("El usuario seleccionado no existe.");
            var partido = await _partidoRepository.GetByIdAsync(dto.PartidoPoliticoId)
                ?? throw new KeyNotFoundException("El partido político seleccionado no existe.");

            if (!user.Estado)
            {
                throw new InvalidOperationException("No se puede asignar un dirigente que se encuentre inactivo.");
            }
            if (!partido.Estado)
            {
                throw new InvalidOperationException("No se puede realizar la asignación a un partido político inactivo.");
            }
            if (user.Rol != "Dirigente político")
            {
                throw new InvalidOperationException("Solo se pueden realizar asignaciones a usuarios con el rol 'Dirigente político'.");
            }

            var asignaciones = await _repository.GetAllAsync();
            if (asignaciones.Any(a => a.UsuarioId == dto.UsuarioId || a.PartidoPoliticoId == dto.PartidoPoliticoId))
            {
                throw new InvalidOperationException("El dirigente o el partido ya tienen una asignación.");
            }

            var entity = new AsignacionDirigente
            {
                UsuarioId = dto.UsuarioId,
                PartidoPoliticoId = dto.PartidoPoliticoId
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            await EnsureNoEleccionActivaAsync("No se pueden eliminar asignaciones mientras exista una elección activa.");

            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }

        public async Task<List<AsignacionDirigenteDto>> GetAllAsync()
        {
            var asignaciones = await _repository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();
            var partidos = await _partidoRepository.GetAllAsync();

            return asignaciones.Select(a => {
                var usuario = usuarios.FirstOrDefault(u => u.Id == a.UsuarioId);
                var partido = partidos.FirstOrDefault(p => p.Id == a.PartidoPoliticoId);

                return new AsignacionDirigenteDto
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    NombreDirigente = usuario != null ? $"{usuario.Nombre} {usuario.Apellido}" : "",
                    NombreUsuario = usuario?.NombreUsuario ?? "",
                    PartidoPoliticoId = a.PartidoPoliticoId,
                    NombrePartido = partido?.Nombre ?? "",
                    SiglasPartido = partido?.Siglas ?? ""
                };
            }).ToList();
        }
    }
}