using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Enums;
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
            return await _context.Elecciones.AnyAsync(e => e.Estado == EstadoEleccion.Activa);
        }
    }
}