using eVote360.Core.Domain.Common;
using eVote360.Core.Domain.Enums;

namespace eVote360.Core.Domain.Entities
{
    public class Eleccion : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public DateTime FechaRealizacion { get; set; }
        public EstadoEleccion Estado { get; set; } = EstadoEleccion.Pendiente;
    }
}