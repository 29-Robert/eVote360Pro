using System;
using System.Linq;
using System.Threading.Tasks;
using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class ElectorService : IElectorService
    {
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepository;
        private readonly IEleccionValidationService _eleccionValidationService;

        public ElectorService(
            IGenericRepository<Ciudadano> ciudadanoRepository,
            IEleccionValidationService eleccionValidationService)
        {
            _ciudadanoRepository = ciudadanoRepository;
            _eleccionValidationService = eleccionValidationService;
        }

        public async Task<string> ValidarElectorInicialAsync(ElectorLoginDto loginDto)
        {
            var EleccionActiva = await _eleccionValidationService.HasEleccionActivaAsync();
            if (!EleccionActiva)
            {
                throw new InvalidOperationException("No hay ningún proceso electoral activo en estos momentos.");
            }

            var ciudadanos = await _ciudadanoRepository.GetAllAsync();
            var elector = ciudadanos.FirstOrDefault(e => e.DocumentoIdentidad == loginDto.DocumentoIdentidad);

            if (elector == null)
            {
                throw new InvalidOperationException("El documento de identidad ingresado no pertenece a ningún ciudadano registrado.");
            }

            if (!elector.Estado)
            {
                throw new InvalidOperationException("Este ciudadano se encuentra inactivo y no está autorizado para votar.");
            }

            return elector.CorreoElectronico;
        }
    }
}
