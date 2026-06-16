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
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Propuesta { get; set; }
        public string NombrePartido { get; set; }
        public string? PuestoAsignado { get; set; }
        public bool EsActivo { get; set; }
        public string? Foto { get; set; }
    }
}
