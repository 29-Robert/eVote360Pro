using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.DTOs.Candidato
{
    public class SaveCandidatodto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El candidato es obligatorio")]
        public int CandidatoId { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(50)]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string CorreoElectronico { get; set; } = null!;

        [Required(ErrorMessage = "El documento es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El documento debe tener 11 dígitos")]
        public string DocumentoIdentidad { get; set; } = null!;


        [Required(ErrorMessage = "El puesto es obligatorio")]
        public int PuestoElectivoId { get; set; }

        public int PartidoPoliticoId { get; set; }

        public bool EsAliado { get; set; } = false;

        public string? Foto { get; set; }

        public bool EsActivo { get; set; }
    }
}
