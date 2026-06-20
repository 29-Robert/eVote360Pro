using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.DTOs.Candidato
{
    public class CandidatoDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Propuesta { get; set; } = string.Empty;
        public string NombrePartido { get; set; } = string.Empty;

        public string? PuestoAsignado { get; set; }
        public bool EsActivo { get; set; }
        public string? Foto { get; set; }

        public string DocumentoIdentidad { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int PartidoPoliticoId { get; set; }
        public int PuestoElectivoId { get; set; }
    }
}