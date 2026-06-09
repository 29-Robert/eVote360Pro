using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class PartidoPolitico : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public bool Estado { get; set; } = true;
    }
}