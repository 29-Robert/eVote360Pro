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
    public class PuestoElectivoRepository : GenericRepository<PuestoElectivo>, IPuestoElectivoRepository
    { 
        private readonly ApplicationDbContext _context;
        public PuestoElectivoRepository(ApplicationDbContext context) : base(context)
        { 
            _context = context;
        }

        public async Task<IEnumerable<PuestoElectivo>> GetActivosAsync()
        {
            return await _context.PuestosElectivos
                .Where(p => p.Estado)
                .ToListAsync();
        }
    }

}
