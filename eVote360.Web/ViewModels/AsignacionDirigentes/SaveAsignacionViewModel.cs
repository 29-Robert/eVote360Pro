using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.AsignacionDirigentes
{
    public class SaveAsignacionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un dirigente político.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un partido político.")]
        public int PartidoPoliticoId { get; set; }
    }
}