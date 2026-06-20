using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;



namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class CandidatoPuestoRepository : ICandidatoPuestoRepository
    {
        private readonly ApplicationDbContext _context;

        public CandidatoPuestoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CandidatoPuesto>> GetByPartidoAsync(int partidoId)
        {
            return await _context.CandidatosPuestos
                .Include(cp => cp.Candidato)
                .Include(cp => cp.PuestoElectivo)
                .Include(cp => cp.PartidoPolitico)
                .Where(cp => cp.PartidoPoliticoId == partidoId)
                .ToListAsync();
        }

        public async Task<CandidatoPuesto> GetByIdAsync(int id)
        {
            return (await _context.CandidatosPuestos
                .Include(cp => cp.Candidato)
                .Include(cp => cp.PuestoElectivo)
                .Include(cp => cp.PartidoPolitico)
                .FirstOrDefaultAsync(cp => cp.Id == id))!;
        }

        public async Task<IEnumerable<CandidatoPuestoDto>> GetCandidatosDisponiblesAsync(int partidoId)
        {
            var asignados = await _context.CandidatosPuestos
             .Where(cp => cp.PartidoPoliticoId == partidoId)
             .Select(cp => cp.CandidatoId)
             .ToListAsync();

            return await _context.Candidatos
                .Include(c => c.PartidoPolitico)
                .Where(c => c.PartidoPoliticoId == partidoId
                         && c.EsActivo
                         && !asignados.Contains(c.Id))
                .Select(c => new CandidatoPuestoDto
                {
                    Id = c.Id,
                    NombreCandidato = c.Nombre,
                    ApellidoCandidato = c.Apellido,
                    FotoCandidato = c.Foto,
                    PartidoPoliticoId = c.PartidoPoliticoId,
                    NombrePartido = c.PartidoPolitico.Nombre
                })
                .ToListAsync();
        }
        

        public async Task<IEnumerable<PuestoElectivoDto>> GetPuestosDisponiblesAsync(int partidoId)
        {

            var ocupados = await _context.CandidatosPuestos
                .Where(cp => cp.PartidoPoliticoId == partidoId)
                .Select(cp => cp.PuestoElectivoId)
                .ToListAsync();

            return await _context.PuestosElectivos
                .Where(p => p.Estado && !ocupados.Contains(p.Id))
                .Select(p => new PuestoElectivoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .ToListAsync();
        }

        public async Task<bool> ExisteCandidatoEnPartidoAsync(int candidatoId, int partidoId)
        {
            return await _context.CandidatosPuestos
                .AnyAsync(cp => cp.CandidatoId == candidatoId
                             && cp.PartidoPoliticoId == partidoId);
        }

        public async Task<bool> ExistePuestoEnPartidoAsync(int puestoId, int partidoId)
        {
            return await _context.CandidatosPuestos
                .AnyAsync(cp => cp.PuestoElectivoId == puestoId
                             && cp.PartidoPoliticoId == partidoId);
        }

        public async Task<int> CountByPartidoAsync(int partidoId)
        {
            return await _context.CandidatosPuestos
                .CountAsync(cp => cp.PartidoPoliticoId == partidoId);
        }

        public async Task AddAsync(CandidatoPuesto asignacion)
        {
            await _context.CandidatosPuestos.AddAsync(asignacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CandidatoPuesto asignacion)
        {
            _context.CandidatosPuestos.Remove(asignacion);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<CandidatoPuesto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}