using eVote360.Core.Application.DTOs.Candidato;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface ICandidatoService
    {
        Task<IEnumerable<CandidatoDTO>> GetByPartidoAsync(int partidoId);
        Task<CandidatoDTO> GetByIdAsync(int id);

        Task AddAsync(SaveCandidatoDto dto);
        Task UpdateAsync(SaveCandidatoDto dto);

        Task ChangeStateAsync(int id, bool estado);
    }
}