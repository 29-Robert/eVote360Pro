using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public bool Estado { get; set; } = true;

        public AsignacionDirigente? AsignacionDirigente { get; set; }
    }
}