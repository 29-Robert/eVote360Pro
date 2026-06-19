using eVote360.Core.Domain.Common;
using eVote360.Core.Domain.Enums;
using System;

namespace eVote360.Core.Domain.Entities
{
    public class SolicitudAlianza : BaseEntity
    {
        public int PartidoSolicitanteId { get; set; }
        public PartidoPolitico? PartidoSolicitante { get; set; }

        public int PartidoReceptorId { get; set; }
        public PartidoPolitico? PartidoReceptor { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public EstadoSolicitudAlianza Estado { get; set; } = EstadoSolicitudAlianza.EnEspera;
    }
}