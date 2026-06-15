using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.DTOs.Candidato
{
    public class CrearCandidatoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La propuesta es obligatoria")]
        public string Propuesta { get; set; }

        public string? Foto { get; set; }
        [Required(ErrorMessage = "La foto es obligatoria")]

        public int PartidoPoliticoId { get; set; }

        public bool EsActivo { get; set; }

    }
}
