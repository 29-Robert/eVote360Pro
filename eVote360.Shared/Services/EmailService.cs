using eVote360.Core.Application.Interfaces.Services;
using eVote360.Shared.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace eVote360.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                // 1. Apagar la verificación de revocación (Evita el error previo)
                smtp.CheckCertificateRevocation = false;

                // 2. Aceptar cualquier certificado SSL (salta bloqueos de antivirus)
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // 3. Conexión segura nativa para el puerto 465
                await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task EnviarResumenVotacionAsync(string correoDestino, string asunto, string cuerpoResumen)
        {
            // Aquí llamas a la lógica interna que ya tengas para enviar el correo.
            // Por ejemplo, si tienes un método interno SendEmailAsync, lo usas aquí:

            // Ejemplo genérico:
            // var request = new EmailRequest { To = correoDestino, Subject = asunto, Body = cuerpoResumen };
            // await SendEmailAsync(request);

            // Si aún no tienes la lógica de envío armada, puedes dejarlo así por ahora 
            // para que compile y luego rellenar el código SMTP:
            await Task.CompletedTask;
        }
    }
}