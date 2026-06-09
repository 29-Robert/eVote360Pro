namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IOcrService
    {
        Task<string> ExtraerTextoDeImagenAsync(string imagePath);
    }
}