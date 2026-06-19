using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.DTOs.Votaciones;
using Microsoft.AspNetCore.Mvc;


namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IElectorService
    {
        Task<string> ValidarElectorInicialAsync(ElectorLoginDto loginDto);
        Task RegistrarVotoAsync(int candidatoId, int puestoId, int eleccionId, int ciudadanoId);
        Task<ValidacionVotacionDto> ValidarFinalizacionAsync(int ciudadanoId, int eleccionId);
        Task MarcarComoVotadoAsync(int ciudadanoId, int eleccionId);
    }
}
