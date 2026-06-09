namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IEleccionValidationService
    {
        Task<bool> HasEleccionActivaAsync();
    }
}