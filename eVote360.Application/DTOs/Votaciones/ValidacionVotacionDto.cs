using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.DTOs.Votaciones
{
    public class ValidacionVotacionDto
    {
        public bool EsValido { get; set; }
        public string PuestosFaltantes { get; set; } = string.Empty;
    }
}
