namespace eVote360.Core.Domain.Entities
{
    public class Voto
    {
        public int Id { get; set; }
        public int CiudadanoId { get; set; }
        public int EleccionId { get; set; }
        public int PuestoElectivoId { get; set; }
        public int? CandidatoId { get; set; } // Puede ser null si eligió "Ninguno"
        public DateTime FechaVoto { get; set; }
    }
}
