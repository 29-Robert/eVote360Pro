using eVote360.Core.Application.DTOs.Alianzas;

namespace eVote360.WebApp.ViewModels.Alianzas
{
    public class AlianzasViewModel
    {
        public IEnumerable<SolicitudAlianzaDto> Pendientes { get; set; }
        = new List<SolicitudAlianzaDto>();

        public IEnumerable<SolicitudAlianzaDto> Realizadas { get; set; }
            = new List<SolicitudAlianzaDto>();

        public IEnumerable<SolicitudAlianzaDto> Vigentes { get; set; }
            = new List<SolicitudAlianzaDto>();
    }
}