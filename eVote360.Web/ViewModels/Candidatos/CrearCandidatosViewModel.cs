using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Candidatos
{
    public class CrearCandidatosViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La foto es obligatoria")]
        public IFormFile Foto { get; set; } = null!;

        public bool EsActivo { get; set; } = true;
    }
}