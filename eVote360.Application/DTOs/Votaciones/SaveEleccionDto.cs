using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.DTOs.Votaciones
{
    public class SaveEleccionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proceso electoral es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de realización es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaRealizacion { get; set; }
    }
}