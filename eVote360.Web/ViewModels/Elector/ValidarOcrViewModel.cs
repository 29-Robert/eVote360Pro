using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Elector
{
    public class ValidarOcrViewModel
    {
        [Display(Name = "Cédula de Identidad")]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, seleccione una foto válida de su cédula.")]
        [Display(Name = "Foto de la Cédula")]
        public IFormFile FotoCedula { get; set; } = null!;
    }
}