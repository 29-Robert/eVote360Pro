using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Candidato : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string DocumentoIdentidad { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string Foto { get; set; } = null!; 
        public bool EsActivo { get; set; } = true;

        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;

        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = null!;
        public ICollection<CandidatoPuesto>? CandidatosPuestos { get; set; }
    }
}