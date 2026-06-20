using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.DTOs.Votaciones;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class ElectorService : IElectorService
    {
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepository;
        private readonly IEleccionValidationService _eleccionValidationService;
        private readonly IVotoRepository _votoRepository;
        private readonly IGenericRepository<PuestoElectivo> _puestoRepository;

        public ElectorService(
            IGenericRepository<Ciudadano> ciudadanoRepository,
            IEleccionValidationService eleccionValidationService,
            IVotoRepository votoRepository,
            IGenericRepository<PuestoElectivo> puestoRepository)
        {
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionValidationService = eleccionValidationService;
            _votoRepository = votoRepository;
            _puestoRepository = puestoRepository;
        }

        public async Task<string> ValidarElectorInicialAsync(ElectorLoginDto loginDto)
        {
            var eleccionActiva = await _eleccionValidationService.HasEleccionActivaAsync();
            if (!eleccionActiva)
            {
                throw new InvalidOperationException("No hay ningún proceso electoral activo en estos momentos.");
            }

            var ciudadanos = await _ciudadanoRepository.GetAllAsync();
            var elector = ciudadanos.FirstOrDefault(e => e.DocumentoIdentidad == loginDto.DocumentoIdentidad);

            if (elector == null)
            {
                throw new InvalidOperationException("El documento de identidad ingresado no pertenece a ningún ciudadano registrado.");
            }

            if (!elector.Estado)
            {
                throw new InvalidOperationException("Este ciudadano se encuentra inactivo y no está autorizado para votar.");
            }


            var yaVoto = await _votoRepository.YaVotoEnEleccionActivaAsync(elector.Id);
            if (yaVoto)
            {
                throw new InvalidOperationException("El ciudadano ya ha registrado su voto en esta elección.");
            }

            return elector.CorreoElectronico;
        }

        public async Task RegistrarVotoAsync(int candidatoId, int puestoId, int eleccionId, int ciudadanoId)
        {
            var votoExistente = await _votoRepository.GetByCiudadanoPuestoAsync(ciudadanoId, puestoId);

            if (votoExistente != null)
            {
                votoExistente.CandidatoId = (candidatoId == 0) ? null : candidatoId;
                await _votoRepository.UpdateAsync(votoExistente);
            }
            else
            {
                await _votoRepository.AddAsync(new Voto
                {
                    CiudadanoId = ciudadanoId,
                    EleccionId = eleccionId,
                    PuestoElectivoId = puestoId,
                    CandidatoId = (candidatoId == 0) ? null : candidatoId,
                    FechaVoto = DateTime.Now
                });
            }
        }

        public async Task<ValidacionVotacionDto> ValidarFinalizacionAsync(int ciudadanoId, int eleccionId)
        {
            var puestos = await _puestoRepository.GetAllAsync();

            var puestosActivos = puestos.Where(p => p.Estado).ToList();

            var votos = await _votoRepository.GetByCiudadanoEleccionAsync(ciudadanoId, eleccionId);
            var puestosVotadosIds = votos.Select(v => v.PuestoElectivoId).ToList();

            var faltantes = puestosActivos.Where(p => !puestosVotadosIds.Contains(p.Id)).Select(p => p.Nombre).ToList();

            return new ValidacionVotacionDto
            {
                EsValido = !faltantes.Any(),
                PuestosFaltantes = string.Join(", ", faltantes)
            };
        }

        public async Task MarcarComoVotadoAsync(int ciudadanoId, int eleccionId)
        {
            var ciudadano = await _ciudadanoRepository.GetByIdAsync(ciudadanoId);
            if (ciudadano != null)
            {

                ciudadano.Estado = false;
                await _ciudadanoRepository.UpdateAsync(ciudadano);
            }
        }
    }
}