using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Usuario> GetByIdWithPartidoAsync(int id)
        {
          
            return await _context.Usuarios
                .Include(u => u.AsignacionDirigente)
                    .ThenInclude(a => a.PartidoPolitico)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> GetByPartidoIdAsync(int partidoId)
        {
           
            return await _context.Usuarios
                .Include(u => u.AsignacionDirigente)
                    .ThenInclude(a => a.PartidoPolitico)
                .FirstOrDefaultAsync(u => u.AsignacionDirigente != null &&
                                          u.AsignacionDirigente.PartidoPoliticoId == partidoId);
        }
    }
}