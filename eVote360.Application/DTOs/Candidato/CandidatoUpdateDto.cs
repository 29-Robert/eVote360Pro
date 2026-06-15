using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.DTOs.Candidato
{
    public class CandidatoUpdateDto : CandidatoDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string? Foto { get; set; } 

        [Required]
        public bool EsActivo { get; set; }

        [Required]
        public int PartidoPoliticoId { get; set; }
    }
}
