using eVote360.Core.Application.DTOs.Alianzas;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IAlianzaService
    {
        Task<IEnumerable<SolicitudAlianzaDto>> GetPendientesAsync(int partidoId);
        Task<IEnumerable<SolicitudAlianzaDto>> GetRealizadasAsync(int partidoId);
        Task<IEnumerable<SolicitudAlianzaDto>> GetVigentesAsync(int partidoId);
        Task CrearSolicitudAsync(int solicitanteId, int receptorId);
        Task AceptarAsync(int solicitudId);
        Task RechazarAsync(int solicitudId);
        Task EliminarSolicitudAsync(int solicitudId);
        Task EliminarAlianzaAsync(int solicitudId);
    }
}