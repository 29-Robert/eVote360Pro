using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task EnviarOtpAsync(int ciudadanoId, int eleccionId, string correoDestino);
        Task<bool> ValidarOtpAsync(int ciudadanoId, string codigo);
    }
}