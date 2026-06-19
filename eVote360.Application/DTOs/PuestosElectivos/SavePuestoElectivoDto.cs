using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.DTOs.PuestosElectivos
{
    public class SavePuestoElectivoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es requerido")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; } = null!;

        public bool Estado { get; set; }
    }
}