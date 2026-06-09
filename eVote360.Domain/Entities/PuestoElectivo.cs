using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class PuestoElectivo : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Estado { get; set; } = true;
    }
}