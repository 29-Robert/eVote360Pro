using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class EleccionRepository : IEleccionRepository
    {
        private readonly ApplicationDbContext _context;
        public EleccionRepository(ApplicationDbContext context) => _context = context;

        public async Task<bool> ExisteEleccionActiva()
        {
            // Lógica para verificar si hay una elección con estado "Activo"
            return await _context.Elecciones.AnyAsync(e => e.Estado == "Activo");
        }
    }
}
