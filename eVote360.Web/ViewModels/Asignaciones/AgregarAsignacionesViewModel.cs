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

        // Propiedades requeridas por el controlador según el error CS1061
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El documento es obligatorio")]
        public string DocumentoIdentidad { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        public string CorreoElectronico { get; set; } = null!;

        public int PartidoPoliticoId { get; set; }



        // Para cargar los selects
        public IEnumerable<SelectListItem> CandidatosDisponibles { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> PuestosDisponibles { get; set; }
            = new List<SelectListItem>();
    }
}