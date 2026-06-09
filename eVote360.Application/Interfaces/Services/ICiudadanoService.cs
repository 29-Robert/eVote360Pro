using eVote360.Core.Application.DTOs.Ciudadanos;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface ICiudadanoService
    {
        Task<SaveCiudadanoDto> AddAsync(SaveCiudadanoDto dto);
        Task UpdateAsync(SaveCiudadanoDto dto);
        Task<List<CiudadanoDto>> GetAllAsync();
        Task<SaveCiudadanoDto?> GetByIdSaveDtoAsync(int id);
        Task ChangeStateAsync(int id, bool state);
    }
}