using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eVote360.Core.Application.DTOs.Resultado; 
using eVote360.Core.Application.Interfaces.Repositories; 
using eVote360.Core.Domain.Enums;
using eVote360.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Application.Services
{
    public class ResultadoService : IResultadoService
    {
        private readonly IGenericRepository<Eleccion> _eleccionRepository;
        private readonly IVotoRepository _votoRepository;
        private readonly ICandidatoRepository _candidatoRepository;

        public ResultadoService(
            IGenericRepository<Eleccion> eleccionRepository,
            IVotoRepository votoRepository,
            ICandidatoRepository candidatoRepository)
        {
            _eleccionRepository = eleccionRepository;
            _votoRepository = votoRepository;
            _candidatoRepository = candidatoRepository;
        }

        public async Task<ResultadoEleccionDto> CalcularResultadosAsync(int eleccionId)
        {
            var eleccion = await _eleccionRepository.GetByIdAsync(eleccionId);

            if (eleccion == null)
            {
                throw new KeyNotFoundException($"El proceso electoral con ID {eleccionId} no existe.");
            }

            if (eleccion.Estado != EstadoEleccion.Finalizada)
            {
                throw new InvalidOperationException(
                    "Seguridad de Escrutinio: No se pueden procesar ni visualizar resultados " +
                    "de un proceso electoral que no se encuentre en estado 'Finalizada'.");
            }

            var resultadoDto = new ResultadoEleccionDto
            {
                EleccionId = eleccion.Id,
                NombreEleccion = eleccion.Nombre,
                FechaFinalizacion = DateTime.UtcNow,
                Candidatos = new List<CandidatoResultadoDto>()
            };

            var votosEleccion = (await _votoRepository.GetByEleccionIdAsync(eleccionId)).ToList();

            resultadoDto.TotalVotosValidos = votosEleccion.Count;
            resultadoDto.TotalVotosBlanco = votosEleccion.Count(v => !v.CandidatoId.HasValue);

            var candidatosResultado = new List<CandidatoResultadoDto>();

            var gruposPorCandidato = votosEleccion
                .Where(v => v.CandidatoId.HasValue)
                .GroupBy(v => v.CandidatoId!.Value);

            foreach (var grupo in gruposPorCandidato)
            {
                var candidato = await _candidatoRepository.GetByIdAsync(grupo.Key);
                var votosObtenidos = grupo.Count();
                var porcentaje = resultadoDto.TotalVotosValidos > 0
                    ? (votosObtenidos * 100.0) / resultadoDto.TotalVotosValidos
                    : 0;

                candidatosResultado.Add(new CandidatoResultadoDto
                {
                    CandidatoId = grupo.Key,
                    NombreCompleto = $"{candidato.Nombre} {candidato.Apellido}",
                    Partido = candidato.PartidoPolitico.Nombre,
                    Siglas = candidato.PartidoPolitico.Siglas,
                    VotosObtenidos = votosObtenidos,
                    Porcentaje = porcentaje
                });
            }

            var candidatosOrdenados = candidatosResultado
                .OrderByDescending(c => c.VotosObtenidos)
                .ToList();

            if (candidatosOrdenados.Count > 0)
            {
                candidatosOrdenados[0].EsGanador = true;
            }

            resultadoDto.Candidatos = candidatosOrdenados;

            return resultadoDto;
        }

        public async Task<IEnumerable<ResumenEleccionAnualDto>> GetResumenAnualAsync(int year)
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            var eleccionesDelAnio = elecciones.Where(e => e.FechaRealizacion.Year == year).ToList();

            var resumenes = new List<ResumenEleccionAnualDto>();

            foreach (var eleccion in eleccionesDelAnio)
            {
                var votos = (await _votoRepository.GetByEleccionIdAsync(eleccion.Id)).ToList();

                var candidateIds = votos
                    .Where(v => v.CandidatoId.HasValue)
                    .Select(v => v.CandidatoId!.Value)
                    .Distinct()
                    .ToList();

                var partyIds = new HashSet<int>();
                foreach (var cId in candidateIds)
                {
                    var candidato = await _candidatoRepository.GetByIdAsync(cId);
                    if (candidato != null)
                    {
                        partyIds.Add(candidato.PartidoPoliticoId);
                    }
                }

                var totalVotantes = votos.Select(v => v.CiudadanoId).Distinct().Count();

                resumenes.Add(new ResumenEleccionAnualDto
                {
                    EleccionId = eleccion.Id,
                    NombreEleccion = eleccion.Nombre,
                    FechaRealizacion = eleccion.FechaRealizacion,
                    Estado = eleccion.Estado.ToString(),
                    CantidadPartidos = partyIds.Count,
                    CantidadCandidatos = candidateIds.Count,
                    CantidadVotantes = totalVotantes
                });
            }

            return resumenes;
        }
    }
}