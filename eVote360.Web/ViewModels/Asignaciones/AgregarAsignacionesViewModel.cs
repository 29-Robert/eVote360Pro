using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360.WebApp.ViewModels.Asignaciones
{
    public class AgregarAsignacionesViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un candidato.")]
        public int CandidatoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un puesto electivo.")]
        public int PuestoElectivoId { get; set; }

        public IEnumerable<SelectListItem>? CandidatosDisponibles { get; set; }
        public IEnumerable<SelectListItem>? PuestosDisponibles { get; set; }
    }
}