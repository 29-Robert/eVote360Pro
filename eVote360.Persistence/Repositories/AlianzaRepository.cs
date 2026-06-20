using eVote360.Core.Application.DTOs.PartidosPoliticos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using eVote360.Core.Domain.Enums;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class AlianzaRepository : IAlianzaRepository
    {
        private readonly ApplicationDbContext _context;

        public AlianzaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SolicitudAlianza>> GetPendientesByReceptorAsync(int partidoId)
        {
            return await _context.SolicitudesAlianzas
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .Where(s => s.PartidoReceptorId == partidoId
                         && s.Estado == EstadoSolicitudAlianza.EnEspera)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitudAlianza>> GetRealizadasBySolicitanteAsync(int partidoId)
        {
            return await _context.SolicitudesAlianzas
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .Where(s => s.PartidoSolicitanteId == partidoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitudAlianza>> GetVigentesByPartidoAsync(int partidoId)
        {
            return await _context.SolicitudesAlianzas
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .Where(s => s.Estado == EstadoSolicitudAlianza.Aceptada &&
                           (s.PartidoSolicitanteId == partidoId ||
                            s.PartidoReceptorId == partidoId))
                .ToListAsync();
        }

        public async Task<IEnumerable<PartidoPoliticoDto>> GetPartidosDisponiblesAsync(int partidoId)
        {

            var noDisponibles = await _context.SolicitudesAlianzas
                .Where(s => (s.PartidoSolicitanteId == partidoId || s.PartidoReceptorId == partidoId)
                         && (s.Estado == EstadoSolicitudAlianza.Aceptada || s.Estado == EstadoSolicitudAlianza.EnEspera))
                .Select(s => s.PartidoSolicitanteId == partidoId
                           ? s.PartidoReceptorId
                           : s.PartidoSolicitanteId)
                .ToListAsync();

            return await _context.PartidosPoliticos
                .Where(p => p.Estado 
                         && p.Id != partidoId
                         && !noDisponibles.Contains(p.Id))
                .Select(p => new PartidoPoliticoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Siglas = p.Siglas
                })
                .ToListAsync();
        }

        public async Task<SolicitudAlianza> GetByIdAsync(int id)
        {
            return await _context.SolicitudesAlianzas
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExisteAlianzaVigenteAsync(int partido1Id, int partido2Id)
        {
            return await _context.SolicitudesAlianzas
                .AnyAsync(s => s.Estado == EstadoSolicitudAlianza.Aceptada &&
                              ((s.PartidoSolicitanteId == partido1Id && s.PartidoReceptorId == partido2Id) ||
                               (s.PartidoSolicitanteId == partido2Id && s.PartidoReceptorId == partido1Id)));
        }

        public async Task<bool> ExisteSolicitudPendienteAsync(int solicitanteId, int receptorId)
        {
            return await _context.SolicitudesAlianzas
                .AnyAsync(s => s.Estado == EstadoSolicitudAlianza.EnEspera &&
                              ((s.PartidoSolicitanteId == solicitanteId && s.PartidoReceptorId == receptorId) ||
                               (s.PartidoSolicitanteId == receptorId && s.PartidoReceptorId == solicitanteId)));
        }

        public async Task<bool> TieneCandidatosAliadosAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudesAlianzas
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            return await _context.CandidatosPuestos
                .AnyAsync(cp => cp.EsAliado &&
                               (cp.PartidoPoliticoId == solicitud.PartidoSolicitanteId ||
                                cp.PartidoPoliticoId == solicitud.PartidoReceptorId));
        }

        public async Task<int> CountVigentesAsync(int partidoId)
        {
            return await _context.SolicitudesAlianzas
                .CountAsync(s => s.Estado == EstadoSolicitudAlianza.Aceptada &&
                                (s.PartidoSolicitanteId == partidoId ||
                                 s.PartidoReceptorId == partidoId));
        }

        public async Task<int> CountPendientesAsync(int partidoId)
        {
            return await _context.SolicitudesAlianzas
                .CountAsync(s => s.Estado == EstadoSolicitudAlianza.EnEspera &&
                                 s.PartidoReceptorId == partidoId);
        }

        public async Task AddAsync(SolicitudAlianza solicitud)
        {
            await _context.SolicitudesAlianzas.AddAsync(solicitud);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SolicitudAlianza solicitud)
        {
            _context.SolicitudesAlianzas.Update(solicitud);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SolicitudAlianza solicitud)
        {
            _context.SolicitudesAlianzas.Remove(solicitud);
            await _context.SaveChangesAsync();
        }
    }
}