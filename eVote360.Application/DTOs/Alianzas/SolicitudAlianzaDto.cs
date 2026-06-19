using System;
using System.Collections.Generic;
using System.Text;
using eVote360.Core.Domain.Enums;

namespace eVote360.Core.Application.DTOs.Alianzas
{
    public class SolicitudAlianzaDto
    {
        public int Id { get; set; }
        public int PartidoSolicitanteId { get; set; }
        public int PartidoReceptorId { get; set; }

        public string NombrePartidoSolicitante { get; set; } = string.Empty;
        public string SiglasPartidoSolicitante { get; set; } = string.Empty;
        public string NombrePartidoReceptor { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }
        public string EstadoNombre => Estado.ToString();
        public string? MotivoRechazo { get; set; }
        public EstadoSolicitudAlianza Estado { get; set; }
    }
}