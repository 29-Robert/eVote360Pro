using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Repositories
{
    public interface IPartidoPoliticoRepository
    {
        Task<PartidoPolitico> GetByIdAsync(int id);
    }
}
