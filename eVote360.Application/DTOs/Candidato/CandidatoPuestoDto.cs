using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eVote360.Core.Application.DTOs.Candidato
{
    public class CandidatoPuestoDto
    {
        public int Id { get; set; }

        // Datos del candidato
        public int CandidatoId { get; set; }

        [Required(ErrorMessage = "El nombre del candidato es obligatorio")]
        public string NombreCandidato { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido del candidato es obligatorio")]
        public string ApellidoCandidato { get; set; } = string.Empty;

        [Required(ErrorMessage = "La foto del candidato es obligatoria")]
        public string FotoCandidato { get; set; } = string.Empty;

        // Datos del puesto
        public int PuestoElectivoId { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es obligatorio")]
        public string NombrePuesto { get; set; } = string.Empty;

        // Datos del partido
        public int PartidoPoliticoId { get; set; }

        [Required(ErrorMessage = "El nombre del partido es obligatorio")]
        public string NombrePartido { get; set; } = string.Empty;

        // Tipo de candidatura
        public bool EsAliado { get; set; }

        public string TipoCandidatura => EsAliado ? "Aliado" : "Propio";
    }
}