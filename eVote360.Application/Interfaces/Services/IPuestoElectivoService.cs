using eVote360.Core.Application.DTOs.PuestosElectivos;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IPuestoElectivoService
    {
        Task<IEnumerable<PuestoElectivoDto>> GetAllAsync();

        Task<SavePuestoElectivoDto> GetByIdSaveDtoAsync(int id);

        Task AddAsync(SavePuestoElectivoDto vm);
        Task UpdateAsync(SavePuestoElectivoDto vm);

        Task ChangeStateAsync(int id);
    }
}