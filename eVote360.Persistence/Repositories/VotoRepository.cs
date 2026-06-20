using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
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

        public async Task<IEnumerable<Voto>> GetByEleccionIdAsync(int eleccionId)
        {
            return await _dbContext.Set<Voto>()
                .Where(v => v.EleccionId == eleccionId)
                .ToListAsync();
        }

        public async Task<bool> YaVotoEnEleccionActivaAsync(int ciudadanoId)
        {
            return await _dbContext.Set<Voto>()
                .AnyAsync(v => v.CiudadanoId == ciudadanoId);
        }

        public async Task<bool> HaParticipadoEnEleccionActivaOFinalizadaAsync(int ciudadanoId)
        {
            return await _dbContext.Set<Voto>()
                .Where(v => v.CiudadanoId == ciudadanoId)
                .Join(
                    _dbContext.Elecciones,
                    voto => voto.EleccionId,
                    eleccion => eleccion.Id,
                    (_, eleccion) => eleccion)
                .AnyAsync(e => e.Estado == EstadoEleccion.Activa || e.Estado == EstadoEleccion.Finalizada);
        }
    }
}