using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface IPuestoElectivoRepository : IGenericRepository<PuestoElectivo>
    {
        Task<IEnumerable<PuestoElectivo>> GetActivosAsync();
    }
}