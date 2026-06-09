namespace eVote360.Core.Application.DTOs.Usuarios
{
    public class SaveUsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string? Password { get; set; }
        public string? ConfirmarPassword { get; set; }
        public string Rol { get; set; } = null!;
    }
}