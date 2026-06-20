using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Home
{
    public class HomePublicaViewModel
    {
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        [Display(Name = "Número de documento de identidad")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "La cédula debe contener exactamente 11 dígitos.")]
        public string DocumentoIdentidad { get; set; } = null!;
    }
}