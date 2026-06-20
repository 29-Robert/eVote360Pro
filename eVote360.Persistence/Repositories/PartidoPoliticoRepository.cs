using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class PartidoPoliticoRepository : IPartidoPoliticoRepository
    {
        private readonly ApplicationDbContext _context;

        public PartidoPoliticoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PartidoPolitico> GetByIdAsync(int id)
        {
            return await _context.PartidosPoliticos
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}