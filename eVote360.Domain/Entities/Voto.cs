namespace eVote360.Core.Domain.Entities
{
    public class Voto
    {
        public int Id { get; set; }
        public int CiudadanoId { get; set; }
        public int EleccionId { get; set; }
        public int PuestoElectivoId { get; set; }
        public int? CandidatoId { get; set; } 
        public DateTime FechaVoto { get; set; }
    }
}
