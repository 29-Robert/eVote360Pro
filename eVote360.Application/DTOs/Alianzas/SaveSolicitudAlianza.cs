using eVote360.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eVote360.Core.Application.DTOs.Alianzas
{
    public class SaveSolicitudAlianzaDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El partido receptor es obligatorio")]
    public int PartidoReceptorId { get; set; }

    public int PartidoSolicitanteId { get; set; } // se toma del dirigente logueado

    public EstadoSolicitudAlianza Estado { get; set; } = EstadoSolicitudAlianza.EnEspera;

    public DateTime FechaSolicitud { get; set; } = DateTime.Now;
}
}
