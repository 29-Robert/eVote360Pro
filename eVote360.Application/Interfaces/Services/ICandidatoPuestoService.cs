using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.DTOs.PuestosElectivos;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface ICandidatoPuestoService
    {
        Task<IEnumerable<CandidatoPuestoDto>> GetByPartidoAsync(int partidoId);
        Task<IEnumerable<CandidatoPuestoDto>> GetCandidatosDisponiblesAsync(int partidoId);
        Task<IEnumerable<PuestoElectivoDto>> GetPuestosDisponiblesAsync(int partidoId);
        Task AgregarAsync(int candidatoId, int puestoId, int partidoId);
        Task EliminarAsync(int candidatoPuestoId);
    }
}
