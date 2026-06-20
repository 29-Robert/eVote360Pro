using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class CandidatoRepository : ICandidatoRepository
    {
        private readonly ApplicationDbContext _context;

        public CandidatoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Candidato>> GetByPartidoAsync(int partidoId)
        {
            return await _context.Candidatos
                .Include(c => c.PartidoPolitico)
                .Include(c => c.PuestoElectivo)
                .Where(c => c.PartidoPoliticoId == partidoId)
                .ToListAsync();
        }

        public async Task<Candidato> GetByIdAsync(int id)
        {
            return await _context.Candidatos
                .Include(c => c.PartidoPolitico)
                .Include(c => c.PuestoElectivo)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Candidato candidato)
        {
            await _context.Candidatos.AddAsync(candidato);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Candidato candidato)
        {
            _context.Candidatos.Update(candidato);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountActivosAsync(int partidoId)
        {
            return await _context.Candidatos
                .CountAsync(c => c.PartidoPoliticoId == partidoId
                              && c.EsActivo == true);
        }

        public async Task<int> CountInactivosAsync(int partidoId)
        {
            return await _context.Candidatos
                .CountAsync(c => c.PartidoPoliticoId == partidoId
                              && c.EsActivo == false);
        }
    }
}