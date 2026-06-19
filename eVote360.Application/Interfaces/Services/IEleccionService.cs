using eVote360.Core.Application.DTOs.Votaciones;
using eVote360.Core.Domain.Entities;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IEleccionService
    {
        Task<IEnumerable<Eleccion>> GetAllAsync();
        Task CrearEleccionAsync(SaveEleccionDto dto);
        Task ActivarEleccionAsync(int id);
        Task FinalizarEleccionAsync(int id);
    }
}
