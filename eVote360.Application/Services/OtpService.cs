using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IGenericRepository<CodigoVerificacion> _otpRepository;
        private readonly IEmailService _emailRepository;

        public OtpService(IGenericRepository<CodigoVerificacion> otpRepository, IEmailService emailService)
        {
            _otpRepository = otpRepository;
            _emailRepository = emailService;
        }

        public async Task EnviarOtpAsync(int ciudadanoId, int eleccionId, string correoDestino)
        {
            Random random = new Random();
            string codigoGenerado = random.Next(100000, 999999).ToString();

            var nuevoOtp = new CodigoVerificacion
            {
                CiudadanoId = ciudadanoId,
                EleccionId = eleccionId,
                Codigo = codigoGenerado,
                FechaGeneracion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddMinutes(5),
                Usado = false
            };

            await _otpRepository.AddAsync(nuevoOtp);

            string asunto = "eVote360 Pro - Tu Código de Verificación de Identidad";
            string cuerpoHtml = $@"
                <div style='font-family: sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 8px; max-width: 500px;'>
                    <h2 style='color: #0d6efd; text-align: center;'>eVote360 Pro</h2>
                    <p>Saludos Estimado(a) Ciudadano(a),</p>
                    <p>Para completar tu acceso al portal de votación y ejercer tu derecho al voto, introduce el siguiente código de un solo uso (OTP):</p>
                    <div style='background-color: #f8f9fa; border: 2px dashed #0d6efd; font-size: 32px; font-weight: bold; text-align: center; padding: 15px; margin: 20px 0; letter-spacing: 5px; color: #333;'>
                        {codigoGenerado}
                    </div>
                    <p style='color: #dc3545; font-size: 13px; text-align: center;'><strong>Importante:</strong> Este código expira en 5 minutos y solo puede ser utilizado una vez.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 11px; color: #777; text-align: center;'>Este es un correo automático, por favor no responda a este mensaje.</p>
                </div>";

            await _emailRepository.SendEmailAsync(correoDestino, asunto, cuerpoHtml);
        }

        public async Task<bool> ValidarOtpAsync(int ciudadanoId, string codigo)
        {
            var codigos = await _otpRepository.GetAllAsync();

            var otpValido = codigos
                .Where(c => c.CiudadanoId == ciudadanoId && c.Codigo == codigo && !c.Usado && c.FechaExpiracion > DateTime.Now)
                .OrderByDescending(c => c.FechaGeneracion)
                .FirstOrDefault();

            if (otpValido == null)
            {
                return false;
            }

            // 🔒 Consumo del token para asegurar un solo uso
            otpValido.Usado = true;
            await _otpRepository.UpdateAsync(otpValido);

            return true;
        }
    }
}