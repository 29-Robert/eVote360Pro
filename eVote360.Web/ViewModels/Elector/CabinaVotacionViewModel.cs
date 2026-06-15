using System.Collections.Generic;
using eVote360.Core.Domain.Entities;

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
        public List<Candidato> Candidatos { get; set; } = new();
    }
}