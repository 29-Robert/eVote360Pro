using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.DTOs.HomeDirigentes
{
    public class HomeDirigentesDto
    {
        public string NombrePartido { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;

        public int CandidatosActivos { get; set; }
        public int CandidatosInactivos { get; set; }
        public int AlianzasVigentes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int CandidatosAsignados { get; set; }
    }
}