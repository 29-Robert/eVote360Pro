namespace eVote360.Core.Application.DTOs.Ciudadanos
{
    public class CiudadanoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string DocumentoIdentidad { get; set; } = null!;
        public bool Estado { get; set; }
    }
}