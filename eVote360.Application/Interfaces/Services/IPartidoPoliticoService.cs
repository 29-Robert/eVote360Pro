using eVote360.Core.Application.DTOs.PartidosPoliticos;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IPartidoPoliticoService
    {
        Task<SavePartidoPoliticoDto> AddAsync(SavePartidoPoliticoDto dto);
        Task UpdateAsync(SavePartidoPoliticoDto dto);
        Task<List<PartidoPoliticoDto>> GetAllAsync();
        Task<SavePartidoPoliticoDto?> GetByIdSaveDtoAsync(int id);

        Task ChangeStateAsync(int id);
    }
}