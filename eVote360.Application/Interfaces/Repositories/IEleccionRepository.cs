using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface IEleccionRepository
    {
        Task<bool> ExisteEleccionActiva();
    }
}