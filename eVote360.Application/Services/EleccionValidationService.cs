using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class EleccionValidationService : IEleccionValidationService
    {
        private readonly IGenericRepository<Eleccion> _repository;

        public EleccionValidationService(IGenericRepository<Eleccion> repository)
        {
            _repository = repository;
        }

        public async Task<bool> HasEleccionActivaAsync()
        {
            var elecciones = await _repository.GetAllAsync();
            return elecciones.Any(e => e.Estado == "Activa");
        }
    }
}