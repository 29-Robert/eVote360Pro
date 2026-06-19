using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.DTOs.Votaciones;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Services
{
    public class ElectorService : IElectorService
    {
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepository;
        private readonly IEleccionValidationService _eleccionValidationService;
        private readonly IVotoRepository _votoRepository;
        private readonly IPuestoElectivoRepository _puestoRepository;

        public ElectorService(
            IGenericRepository<Ciudadano> ciudadanoRepository,
            IEleccionValidationService eleccionValidationService,
            IVotoRepository votoRepository,
            IPuestoElectivoRepository puestoRepository)
        {
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionValidationService = eleccionValidationService;
            _votoRepository = votoRepository;
            _puestoRepository = puestoRepository;
        }

        public async Task<string> ValidarElectorInicialAsync(ElectorLoginDto loginDto)
        {
            var EleccionActiva = await _eleccionValidationService.HasEleccionActivaAsync();
            if (!EleccionActiva)
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

            return elector.CorreoElectronico;
        }

        public async Task RegistrarVotoAsync(int candidatoId, int puestoId, int eleccionId, int ciudadanoId)
        {
            // 1. Verificamos si ya existe un voto para este puesto y este ciudadano
            var votoExistente = await _votoRepository.GetByCiudadanoPuestoAsync(ciudadanoId, puestoId);

            if (votoExistente != null)
            {
                // Si ya votó, actualizamos (modificamos) el voto anterior
                votoExistente.CandidatoId = (candidatoId == 0) ? null : candidatoId;
                await _votoRepository.UpdateAsync(votoExistente);
            }
            else
            {
                // Si es nuevo, creamos el registro
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
            // Lógica para verificar si faltan puestos por votar
            var puestos = await _puestoRepository.GetAllAsync();
            var votos = await _votoRepository.GetByCiudadanoEleccionAsync(ciudadanoId, eleccionId);

            var puestosVotadosIds = votos.Select(v => v.PuestoElectivoId).ToList();
            var faltantes = puestos.Where(p => !puestosVotadosIds.Contains(p.Id)).Select(p => p.Nombre).ToList();

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
                ciudadano.YaVoto = true;
                await _ciudadanoRepository.UpdateAsync(ciudadano);
            }
        }
    }
}
