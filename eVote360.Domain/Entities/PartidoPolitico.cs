using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class PartidoPolitico : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public bool Estado { get; set; } = true;

        public ICollection<SolicitudAlianza> SolicitudesEnviadas { get; set; } = new List<SolicitudAlianza>();
        public ICollection<SolicitudAlianza> SolicitudesRecibidas { get; set; } = new List<SolicitudAlianza>();
        public ICollection<CandidatoPuesto> CandidatosPuestos { get; set; } = new List<CandidatoPuesto>();
        public ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    }
}