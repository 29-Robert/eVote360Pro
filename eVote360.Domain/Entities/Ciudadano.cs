using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Ciudadano : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string DocumentoIdentidad { get; set; } = null!;
        public bool Estado { get; set; } = true;
        public bool YaVoto { get; set; }
    }
}