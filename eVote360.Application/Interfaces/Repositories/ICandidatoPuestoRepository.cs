using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface ICandidatoPuestoRepository
    {
        Task<IEnumerable<CandidatoPuesto>> GetByPartidoAsync(int partidoId);
        Task<CandidatoPuesto> GetByIdAsync(int id);
        Task<IEnumerable<CandidatoPuestoDto>> GetCandidatosDisponiblesAsync(int partidoId);
        Task<IEnumerable<PuestoElectivoDto>> GetPuestosDisponiblesAsync(int partidoId);
        Task<bool> ExisteCandidatoEnPartidoAsync(int candidatoId, int partidoId);
        Task<bool> ExistePuestoEnPartidoAsync(int puestoId, int partidoId);
        Task<int> CountByPartidoAsync(int partidoId);
        Task AddAsync(CandidatoPuesto asignacion);
        Task DeleteAsync(CandidatoPuesto asignacion);
    }
}
