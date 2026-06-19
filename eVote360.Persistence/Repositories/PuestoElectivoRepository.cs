using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class PuestoElectivoRepository : GenericRepository<PuestoElectivo>, IPuestoElectivoRepository
    {
        public PuestoElectivoRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
