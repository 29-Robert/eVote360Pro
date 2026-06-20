using eVote360.Core.Application.DTOs.PartidosPoliticos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IPartidoPoliticoService
    {
        Task<SavePartidoPoliticoDto> AddAsync(SavePartidoPoliticoDto dto);
        Task UpdateAsync(SavePartidoPoliticoDto dto);
        Task<List<PartidoPoliticoDto>> GetAllAsync();
        Task<SavePartidoPoliticoDto?> GetByIdSaveDtoAsync(int id);
        Task ChangeStateAsync(int id, bool state);
        Task<bool> PuedeModificarPartidoAsync(int partidoId);
    }
}