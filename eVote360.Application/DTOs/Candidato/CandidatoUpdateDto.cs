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
        public new int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public new string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public new string Apellido { get; set; } = string.Empty;

        // Si la foto es obligatoria por validación, no debería ser string?
        [Required(ErrorMessage = "La foto es obligatoria")]
        public new string Foto { get; set; } = string.Empty;

        [Required]
        public new bool EsActivo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un partido")]
        public int PartidoPoliticoId { get; set; }
    }
}