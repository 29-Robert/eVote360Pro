using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class VotoRepository : GenericRepository<Voto>, IVotoRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VotoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Voto> GetByCiudadanoPuestoAsync(int ciudadanoId, int puestoId)
        {
            return (await _dbContext.Set<Voto>()
                .FirstOrDefaultAsync(v => v.CiudadanoId == ciudadanoId && v.PuestoElectivoId == puestoId))!;
        }

        public async Task<IEnumerable<Voto>> GetByCiudadanoEleccionAsync(int ciudadanoId, int eleccionId)
        {
            return await _dbContext.Set<Voto>()
                .Where(v => v.CiudadanoId == ciudadanoId && v.EleccionId == eleccionId)
                .ToListAsync();
        }

        // 🛠️ AGREGA ESTE MÉTODO PARA SOLUCIONAR EL ERROR CS0535
        public async Task<bool> YaVotoEnEleccionActivaAsync(int ciudadanoId)
        {
            // Retorna true si el ciudadano ya tiene al menos un voto registrado en el sistema
            return await _dbContext.Set<Voto>()
                .AnyAsync(v => v.CiudadanoId == ciudadanoId);
        }
    }
}