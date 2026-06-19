using System.Threading.Tasks;
using eVote360.Core.Application.DTOs.Resultado; 

namespace eVote360.Application.Interfaces.Services
{
    public interface IResultadoService
    {
        Task<ResultadoEleccionDto> CalcularResultadosAsync(int eleccionId);
    }
}