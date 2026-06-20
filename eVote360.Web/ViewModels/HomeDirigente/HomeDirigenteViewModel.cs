namespace eVote360.WebApp.ViewModels.HomeDirigente
{
    public class HomeDirigenteViewModel
    {
        public string NombreCompleto { get; set; } = null!;
        public string NombrePartido { get; set; } = null!;
        public string SiglasPartido { get; set; } = null!;
        public bool PartidoActivo { get; set; }

        public int TotalCandidatos { get; set; }
        public int CandidatosActivos { get; set; }
        public int AlianzasPendientes { get; set; }
        public int CandidatosAsignados { get; set; }

    }    
}