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
            // 1. si ya existe otra elección activa en el sistema
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.Estado == EstadoEleccion.Activa))
            {
                throw new InvalidOperationException("Ya existe un proceso electoral activo. Debe finalizarlo antes de activar uno nuevo.");
            }

            // 2. Validar la Regla de Negocio: Partidos sin candidatos
            var partidos = await _partidoRepository.GetAllAsync();
            var candidatos = await _candidatoRepository.GetAllAsync();

            foreach (var partido in partidos)
            {
                // Verificamos si el partido tiene al menos un candidato inscrito
                bool tieneCandidatos = candidatos.Any(c => c.PartidoPoliticoId == partido.Id); // Ajustar propiedad según tu entidad
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
    }
}