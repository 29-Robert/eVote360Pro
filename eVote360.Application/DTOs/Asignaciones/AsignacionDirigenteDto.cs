namespace eVote360.Core.Application.DTOs.Asignaciones
{
    public class AsignacionDirigenteDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreDirigente { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public int PartidoPoliticoId { get; set; }
        public string NombrePartido { get; set; } = null!;
        public string SiglasPartido { get; set; } = null!;
    }
}