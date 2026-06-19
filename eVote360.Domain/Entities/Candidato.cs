
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Candidato : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Foto { get; set; } = null!;
        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;
        public int? PuestoElectivoId { get; set; }
        public PuestoElectivo? PuestoElectivo { get; set; }
        public bool EsActivo { get; set; }

        public ICollection<CandidatoPuesto> CandidatosPuestos { get; set; } = new List<CandidatoPuesto>();
    }
}