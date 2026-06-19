namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task EnviarResumenVotacionAsync(string correoDestino, string asunto, string cuerpoResumen);
    }
}