using eVote360.Core.Application.DTOs.Ciudadanos;


namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IElectorService
    {
        Task<string> ValidarElectorInicialAsync(ElectorLoginDto loginDto);
    }
}
