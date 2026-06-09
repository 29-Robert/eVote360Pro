using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class AsignacionDirigente : BaseEntity
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;
    }
}