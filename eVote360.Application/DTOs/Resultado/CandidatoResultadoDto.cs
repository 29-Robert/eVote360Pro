namespace eVote360.Core.Application.DTOs.Resultado
{
    public class CandidatoResultadoDto
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Partido { get; set; } = null!;
        public string Siglas { get; set; } = null!;

        public int VotosObtenidos { get; set; }
        public double Porcentaje { get; set; }
        public bool EsGanador { get; set; }
    }
}