using eVote360.Core.Application.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace eVote360.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.mailtrap.io")
            {
                Port = 2525,
                Credentials = new NetworkCredential("tu_usuario", "tu_contraseña"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@evote360.edu.do", "eVote360 Pro Motor Electoral"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);// de donde el metodo ?
            }
            catch
            {
                throw new Exception("No fue posible enviar el código de verificación. Intente nuevamente más tarde."); //
            }
        }
    }
}
