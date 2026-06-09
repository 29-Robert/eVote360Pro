using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.PuestosElectivos
{
    public class SavePuestoElectivoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es requerido")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; } = null!;
    }
}