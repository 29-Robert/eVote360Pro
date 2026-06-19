using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Ciudadanos
{
    public class SaveCiudadanoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe introducir una dirección de correo válida.")]
        public string CorreoElectronico { get; set; } = null!;

        [Required(ErrorMessage = "El documento de identidad (Cédula) es obligatorio.")]
        [RegularExpression(@"^\d{3}-?\d{7}-?\d{1}$", ErrorMessage = "El formato de la cédula debe ser 000-0000000-0.")]
        public string DocumentoIdentidad { get; set; } = null!;
    }
}