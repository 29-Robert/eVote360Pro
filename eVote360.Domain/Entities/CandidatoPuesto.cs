using eVote360.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Domain.Entities
{
    public class CandidatoPuesto : BaseEntity
    {
        public int CandidatoId { get; set; }
        public Candidato Candidato { get; set; } = null!;

        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = null!;

        public int PartidoPoliticoId { get; set; }
        public PartidoPolitico PartidoPolitico { get; set; } = null!;

        public bool EsAliado { get; set; }
    }
}