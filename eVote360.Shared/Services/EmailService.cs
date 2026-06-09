using eVote360.Core.Application.Interfaces.Services;

namespace eVote360.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await Task.CompletedTask;
        }
    }
}