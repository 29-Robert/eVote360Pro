using eVote360.Core.Application.DTOs.Asignaciones;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IAsignacionDirigenteService
    {
        Task<SaveAsignacionDirigenteDto> AddAsync(SaveAsignacionDirigenteDto dto);
        Task DeleteAsync(int id);
        Task<List<AsignacionDirigenteDto>> GetAllAsync();
    }
}