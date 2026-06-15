using eVote360.Core.Domain.Common;
using System;

namespace eVote360.Core.Domain.Entities
{
    public class CodigoVerificacion : BaseEntity
    {
        public int CiudadanoId { get; set; } 
        public int EleccionId { get; set; }
        public string Codigo { get; set; } = null!;  
        public DateTime FechaGeneracion { get; set; } 
        public DateTime FechaExpiracion { get; set; } 
        public bool Usado { get; set; } 
    }
}
