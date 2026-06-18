using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.PartidosPoliticos
{
    public class SavePartidoPoliticoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del partido es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Las siglas son obligatorias.")]
        public string Siglas { get; set; } = null!;

        public string? Descripcion { get; set; }

        public IFormFile? LogoFile { get; set; }

        public string? LogoUrl { get; set; }
    }
}