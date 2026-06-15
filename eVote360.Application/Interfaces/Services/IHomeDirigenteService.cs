using eVote360.Core.Application.DTOs.HomeDirigentes;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Interfaces.Services
{
    public interface IHomeDirigenteService
    {
        Task<HomeDirigentesDto> GetIndicadoresAsync(int partidoId);
    }
}
