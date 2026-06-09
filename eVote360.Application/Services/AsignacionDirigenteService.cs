using eVote360.Core.Application.DTOs.Asignaciones;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class AsignacionDirigenteService : IAsignacionDirigenteService
    {
        private readonly IGenericRepository<AsignacionDirigente> _repository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IGenericRepository<PartidoPolitico> _partidoRepository;

        public AsignacionDirigenteService(
            IGenericRepository<AsignacionDirigente> repository,
            IGenericRepository<Usuario> usuarioRepository,
            IGenericRepository<PartidoPolitico> partidoRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _partidoRepository = partidoRepository;
        }

        public async Task<SaveAsignacionDirigenteDto> AddAsync(SaveAsignacionDirigenteDto dto)
        {
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