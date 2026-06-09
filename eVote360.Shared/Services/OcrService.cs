using eVote360.Core.Application.Interfaces.Services;

namespace eVote360.Infrastructure.Shared.Services
{
    public class OcrService : IOcrService
    {
        public async Task<string> ExtraerTextoDeImagenAsync(string imagePath)
        {
            return await Task.FromResult(string.Empty);
        }
    }
}