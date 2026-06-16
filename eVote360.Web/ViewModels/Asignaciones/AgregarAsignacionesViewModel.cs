using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Asignaciones
{
    public class AgregarAsignacionesViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un candidato")]
        public int CandidatoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un puesto")]
        public int PuestoElectivoId { get; set; }

        // Para cargar los selects
        public IEnumerable<SelectListItem> CandidatosDisponibles { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> PuestosDisponibles { get; set; }
            = new List<SelectListItem>();
    }
}
