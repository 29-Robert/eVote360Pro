using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.DTOs.Candidato
{
    public class SaveCandidatoDto
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [Required]
        public string DocumentoIdentidad { get; set; } = null!;

        [Required]
        public string CorreoElectronico { get; set; } = null!;

        [Required]
        public int PartidoPoliticoId { get; set; }

        [Required]
        public int PuestoElectivoId { get; set; }

        public string? Foto { get; set; }
    }
}