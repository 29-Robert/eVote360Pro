using System.IO;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IOcrService
    {
        Task<string> ExtraerTextoDeImagenAsync(Stream imagenStream);
    }
}