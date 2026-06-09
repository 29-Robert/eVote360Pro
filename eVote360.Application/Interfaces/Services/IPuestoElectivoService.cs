using eVote360.Core.Application.DTOs.PuestosElectivos;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IPuestoElectivoService
    {
        Task<SavePuestoElectivoDto> AddAsync(SavePuestoElectivoDto dto);
        Task UpdateAsync(SavePuestoElectivoDto dto);
        Task<List<PuestoElectivoDto>> GetAllAsync();
        Task<SavePuestoElectivoDto?> GetByIdSaveDtoAsync(int id);
        Task ChangeStateAsync(int id, bool state);
    }
}