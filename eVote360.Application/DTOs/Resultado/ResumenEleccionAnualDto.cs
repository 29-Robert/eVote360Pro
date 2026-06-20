using System;

namespace eVote360.Core.Application.DTOs.Resultado
{
    public class ResumenEleccionAnualDto
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = null!;
        public DateTime FechaRealizacion { get; set; }
        public string Estado { get; set; } = null!;
        public int CantidadPartidos { get; set; }
        public int CantidadCandidatos { get; set; }
        public int CantidadVotantes { get; set; }
    }
}