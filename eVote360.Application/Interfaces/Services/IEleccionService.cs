using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IEleccionService
    {
        Task ActivarEleccionAsync(int id);
        Task FinalizarEleccionAsync(int id);
    }
}
