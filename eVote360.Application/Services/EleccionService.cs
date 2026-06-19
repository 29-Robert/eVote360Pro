using System;
using System.Linq;
using System.Threading.Tasks;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;

namespace eVote360.Core.Application.Services
{
    public class EleccionService : IEleccionService
    {
        private readonly IGenericRepository<Eleccion> _eleccionRepository;
        private readonly IGenericRepository<PartidoPolitico> _partidoRepository;
        private readonly IGenericRepository<Candidato> _candidatoRepository; 

        public EleccionService(
            IGenericRepository<Eleccion> eleccionRepository,
            IGenericRepository<PartidoPolitico> partidoRepository,
            IGenericRepository<Candidato> candidatoRepository)
        {
            _eleccionRepository = eleccionRepository;
            _partidoRepository = partidoRepository;
            _candidatoRepository = candidatoRepository;
        }

        public async Task ActivarEleccionAsync(int id)
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.Estado == EstadoEleccion.Activa))
            {
                throw new InvalidOperationException("Ya existe un proceso electoral activo. Debe finalizarlo antes de activar uno nuevo.");
            }

            var partidos = await _partidoRepository.GetAllAsync();
            var candidatos = await _candidatoRepository.GetAllAsync();

            foreach (var partido in partidos)
            {
                bool tieneCandidatos = candidatos.Any(c => c.PartidoPoliticoId == partido.Id); 
                if (!tieneCandidatos)
                {
                    throw new InvalidOperationException($"No se puede activar la elección. El partido '{partido.Nombre}' no tiene ningún candidato asignado.");
                }
            }

            // 3. Cambiar el estado a Activa
            var eleccion = await _eleccionRepository.GetByIdAsync(id);
            if (eleccion != null)
            {
                eleccion.Estado = EstadoEleccion.Activa;
                await _eleccionRepository.UpdateAsync(eleccion);
            }
        }

        public async Task FinalizarEleccionAsync(int id)
        {
            var eleccion = await _eleccionRepository.GetByIdAsync(id);
            if (eleccion != null)
            {
                eleccion.Estado = EstadoEleccion.Finalizada;
                await _eleccionRepository.UpdateAsync(eleccion);
            }
        }

        public async Task CambiarEstadoAActivaAsync(int eleccionId)
        {
            // 1. Desactivar cualquier elección que esté activa actualmente
            var eleccionesActivas = await _eleccionRepository.GetAllAsync();
            var activa = eleccionesActivas.FirstOrDefault(e => e.Estado == EstadoEleccion.Activa);

            if (activa != null)
            {
                activa.Estado = EstadoEleccion.Finalizada; // O Pendiente, según tu flujo
                await _eleccionRepository.UpdateAsync(activa);
            }

            // 2. Activar la nueva elección
            var nueva = await _eleccionRepository.GetByIdAsync(eleccionId);
            if (nueva != null)
            {
                nueva.Estado = EstadoEleccion.Activa;
                await _eleccionRepository.UpdateAsync(nueva);
            }
        }
    }
}