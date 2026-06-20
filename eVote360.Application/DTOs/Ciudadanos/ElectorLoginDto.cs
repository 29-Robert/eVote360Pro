using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.DTOs.Ciudadanos
{
    public class ElectorLoginDto
    {
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        [Display(Name = "Número de documento de identidad")]
        public string DocumentoIdentidad { get; set; } = null!;
    }
}