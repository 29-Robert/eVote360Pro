using System;
using System.Collections.Generic;

namespace eVote360.Core.Application.DTOs.Resultado
{
    public class ResultadoEleccionDto
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = null!;
        public DateTime FechaFinalizacion { get; set; }

        public int TotalVotosValidos { get; set; }
        public int TotalVotosBlanco { get; set; }

        public List<CandidatoResultadoDto> Candidatos { get; set; } = new();
    }
}