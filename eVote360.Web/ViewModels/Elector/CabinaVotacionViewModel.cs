using System.Collections.Generic;

namespace eVote360.WebApp.ViewModels.Elector
{
    public class CabinaVotacionViewModel
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = null!;
        public List<PuestoVotacionDto> Puestos { get; set; } = new();
    }

    public class PuestoVotacionDto
    {
        public int PuestoId { get; set; }
        public string NombrePuesto { get; set; } = null!;
        public int? CandidatoSeleccionadoId { get; set; }
        public bool SeleccionadoNinguno { get; set; }
        public List<CandidatoVotacionDto> Candidatos { get; set; } = new();
    }

    public class CandidatoVotacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Foto { get; set; } = null!;
        public string NombrePartido { get; set; } = null!;
        public string SiglasPartido { get; set; } = null!;
    }
}