using eVote360.Core.Domain.Entities;
using eVote360.Core.Application.Interfaces.Repositories;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface IVotoRepository : IGenericRepository<Voto>
    {
        Task<Voto> GetByCiudadanoPuestoAsync(int ciudadanoId, int puestoId);
        Task<IEnumerable<Voto>> GetByCiudadanoEleccionAsync(int ciudadanoId, int eleccionId);
        Task<IEnumerable<Voto>> GetByEleccionIdAsync(int eleccionId);
        Task<bool> YaVotoEnEleccionActivaAsync(int ciudadanoId);
    }
}