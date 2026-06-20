using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Ciudadanos
{
    public class SaveCiudadanoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string CorreoElectronico { get; set; } = null!;

        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        [Display(Name = "Número de documento de identidad")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El número de documento de identidad solo debe contener dígitos.")]
        public string DocumentoIdentidad { get; set; } = null!;

        [Display(Name = "Activo")]
        public bool Estado { get; set; } = true;

        public bool PuedeEditarDocumentoIdentidad { get; set; } = true;
    }
}