using eVote360.Core.Application.DTOs.Candidato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface ICandidatoService
    {
        
            Task<IEnumerable<CandidatoDTO>> GetByPartidoAsync(int partidoId);
            Task<CandidatoDTO> GetByIdAsync(int id);
            Task CreateAsync(CrearCandidatoDto model);
            Task UpdateAsync(CandidatoUpdateDto model);
            Task ActivarAsync(int id);
            Task DesactivarAsync(int id);
    }
}
