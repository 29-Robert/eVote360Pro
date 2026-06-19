namespace eVote360.WebApp.ViewModels.Elector
{
    public class CabinaVotacionViewModel
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = null!;
        public List<PartidoConCandidatosDto> Partidos { get; set; } = new();
    }

    public class PartidoConCandidatosDto
    {
        public int PartidoId { get; set; }
        public string NombrePartido { get; set; } = null!;
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public List<CandidatoConPuestoDto> Candidatos { get; set; } = new();
    }

    public class CandidatoConPuestoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Foto { get; set; } = null!;
        public int PuestoElectivoId { get; set; } 
    }
}