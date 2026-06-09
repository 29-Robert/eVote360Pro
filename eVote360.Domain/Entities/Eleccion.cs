using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Eleccion : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public DateTime FechaRealizacion { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}