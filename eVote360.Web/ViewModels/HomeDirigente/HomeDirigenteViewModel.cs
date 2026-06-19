namespace eVote360.WebApp.ViewModels.HomeDirigente
{
    public class HomeDirigenteViewModel
    {
        // --- Propiedades originales (Las mantenemos) ---
        public string NombreCompleto { get; set; } = null!;
        public string NombrePartido { get; set; } = null!;
        public string SiglasPartido { get; set; } = null!;
        public bool PartidoActivo { get; set; }

        public int TotalCandidatos { get; set; }
        public int CandidatosActivos { get; set; }
        public int AlianzasPendientes { get; set; }
        public int CandidatosAsignados { get; set; }

        // --- NUEVAS propiedades que exige el DirigenteController ---
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public int PartidoPoliticoId { get; set; }
        public int AlianzasAceptadas { get; set; }
    }
}