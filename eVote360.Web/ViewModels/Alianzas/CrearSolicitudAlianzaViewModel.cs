using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Alianzas
{
    public class CrearSolicitudAlianzaViewModel
    {
        
        
        [Required(ErrorMessage = "Debe seleccionar un partido")]
        public int PartidoReceptorId { get; set; }

    
        public IEnumerable<SelectListItem> PartidosDisponibles { get; set; }
            = new List<SelectListItem>();
    }
}