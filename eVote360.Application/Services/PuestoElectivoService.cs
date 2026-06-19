using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;

namespace eVote360.Core.Application.Services
{
    public class PuestoElectivoService : IPuestoElectivoService
    {
        private readonly IPuestoElectivoRepository _puestoRepository;
        private readonly IGenericRepository<Eleccion> _eleccionRepository;
        private readonly ICandidatoPuestoRepository _candidatoPuestoRepository;
        private readonly IPuestoElectivoRepository _repository; 

        public PuestoElectivoService(
            IPuestoElectivoRepository puestoRepository,
            IGenericRepository<Eleccion> eleccionRepository,
            ICandidatoPuestoRepository candidatoPuestoRepository,
            IPuestoElectivoRepository repository)
        {
            _puestoRepository = puestoRepository;
            _eleccionRepository = eleccionRepository;
            _candidatoPuestoRepository = candidatoPuestoRepository;
            _repository = repository;
        }

        private async Task ValidarEleccionActivaAsync()
        {
            var elecciones = await _eleccionRepository.GetAllAsync();
            if (elecciones.Any(e => e.Estado == EstadoEleccion.Activa))
            {
                throw new InvalidOperationException("No se pueden modificar puestos electivos mientras exista una elección activa.");
            }
        }

        public async Task<IEnumerable<PuestoElectivoDto>> GetAllAsync()
        {
            var puestos = await _puestoRepository.GetAllAsync();
            return puestos.Select(p => new PuestoElectivoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Estado = p.Estado
            }).ToList();
        }

        public async Task<SavePuestoElectivoDto> GetByIdSaveDtoAsync(int id)
        {
            var puesto = await _puestoRepository.GetByIdAsync(id);
            if (puesto == null) return null!;

            return new SavePuestoElectivoDto
            {
                Id = puesto.Id,
                Nombre = puesto.Nombre,
                Descripcion = puesto.Descripcion,
                Estado = puesto.Estado
            };
        }

        public async Task AddAsync(SavePuestoElectivoDto vm)
        {
            await ValidarEleccionActivaAsync();

            var puestos = await _puestoRepository.GetAllAsync();
            if (puestos.Any(p => p.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                throw new InvalidOperationException("Ya existe un puesto electivo registrado con este nombre.");
            }

            var puesto = new PuestoElectivo
            {
                Nombre = vm.Nombre.Trim(),
                Descripcion = vm.Descripcion,
                Estado = true
            };

            await _puestoRepository.AddAsync(puesto);
        }

        public async Task UpdateAsync(SavePuestoElectivoDto vm)
        {
            await ValidarEleccionActivaAsync();

            var puesto = await _puestoRepository.GetByIdAsync(vm.Id);
            if (puesto == null) return;

            var puestos = await _puestoRepository.GetAllAsync();
            if (puestos.Any(p => p.Id != vm.Id && p.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                throw new InvalidOperationException("Ya existe un puesto electivo registrado con este nombre.");
            }

            puesto.Nombre = vm.Nombre.Trim();
            puesto.Descripcion = vm.Descripcion;
            puesto.Estado = vm.Estado;

            await _puestoRepository.UpdateAsync(puesto);
        }

        public async Task ChangeStateAsync(int id, bool state)
        {
            var puesto = await _repository.GetByIdAsync(id);

            if (puesto == null)
            {
                throw new Exception("El puesto electivo no fue encontrado.");
            }

            puesto.Estado = state;

            await _repository.UpdateAsync(puesto);
        }
    }
}