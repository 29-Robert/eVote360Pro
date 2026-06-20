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
                smtp.CheckCertificateRevocation = false;

                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (System.Exception ex)
            {

                System.Console.WriteLine($"[SMTP SIMULATION] Failed to send email to {to}. Subject: '{subject}'. Error: {ex.Message}");
            }
            finally
            {
                try
                {
                    await smtp.DisconnectAsync(true);
                }
                catch
                {

                }
            }
        }

        public async Task EnviarResumenVotacionAsync(string correoDestino, string asunto, string cuerpoResumen)
        {
            await SendEmailAsync(correoDestino, asunto, cuerpoResumen);
        }
    }
}