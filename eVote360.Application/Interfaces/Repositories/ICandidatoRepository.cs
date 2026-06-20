using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface ICandidatoRepository
    {
        Task<IEnumerable<Candidato>> GetByPartidoAsync(int partidoId);
        Task<Candidato> GetByIdAsync(int id);
        Task AddAsync(Candidato candidato);
        Task UpdateAsync(Candidato candidato);
        Task<int> CountActivosAsync(int partidoId);
        Task<int> CountInactivosAsync(int partidoId);
    }
}