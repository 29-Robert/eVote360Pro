using eVote360.Core.Application.DTOs.Usuarios;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<SaveUsuarioDto> AddAsync(SaveUsuarioDto dto);
        Task UpdateAsync(SaveUsuarioDto dto);
        Task<List<UsuarioDto>> GetAllAsync();
        Task<SaveUsuarioDto?> GetByIdSaveDtoAsync(int id);
        Task ChangeStateAsync(int id, bool state);
        Task<UsuarioDto?> LoginAsync(LoginDto dto);
    }
}