using eVote360.Core.Application.DTOs.PartidosPoliticos;
using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface IAlianzaRepository
    {
        Task<IEnumerable<SolicitudAlianza>> GetPendientesByReceptorAsync(int partidoId);
        Task<IEnumerable<SolicitudAlianza>> GetRealizadasBySolicitanteAsync(int partidoId);
        Task<IEnumerable<SolicitudAlianza>> GetVigentesByPartidoAsync(int partidoId);
        Task<IEnumerable<PartidoPoliticoDto>> GetPartidosDisponiblesAsync(int partidoId);
        Task<SolicitudAlianza> GetByIdAsync(int id);
        Task<bool> ExisteAlianzaVigenteAsync(int partido1Id, int partido2Id);
        Task<bool> ExisteSolicitudPendienteAsync(int solicitanteId, int receptorId);
        Task<bool> TieneCandidatosAliadosAsync(int solicitudId);
        Task<int> CountVigentesAsync(int partidoId);
        Task<int> CountPendientesAsync(int partidoId);
        Task AddAsync(SolicitudAlianza solicitud);
        Task UpdateAsync(SolicitudAlianza solicitud);
        Task DeleteAsync(SolicitudAlianza solicitud);
    }
}
