using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.DTOs.PuestosElectivos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Services
{
    public class CandidatoPuestoService : ICandidatoPuestoService
    {
        private readonly ICandidatoPuestoRepository _repo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CandidatoPuestoService(ICandidatoPuestoRepository repo, IEleccionRepository eleccionRepo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _eleccionRepo = eleccionRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AgregarAsync(int CandidatoId, int PuestoId, int PartidoId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede asignar candidatos mientras exista una elección activa.");

            if (await _repo.ExisteCandidatoEnPartidoAsync(CandidatoId, PartidoId))
                throw new Exception("Este candidato ya está asignado a un puesto dentro del partido.");

            if (await _repo.ExistePuestoEnPartidoAsync(PuestoId, PartidoId))
                throw new Exception("Este puesto electivo ya tiene un candidato asignado dentro del partido.");

            var asignacion = new CandidatoPuesto
            {
                CandidatoId = CandidatoId,
                PuestoElectivoId = PuestoId,
                PartidoPoliticoId = PartidoId,
                EsAliado = false
            };

            await _repo.AddAsync(asignacion);
        }

        public async Task EliminarAsync(int CandidatoPuestoId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede eliminar una asignación mientras exista una elección activa.");

            var asignacion = await _repo.GetByIdAsync(CandidatoPuestoId);

            var claim = _httpContextAccessor.HttpContext.User.FindFirst("PartidoId");
            if (claim == null) throw new Exception("No se pudo identificar al usuario.");

            int PartidoId = int.Parse(claim.Value);

            if (asignacion.PartidoPoliticoId != PartidoId)
                throw new Exception("No tiene permisos para eliminar esta asignación.");

            await _repo.DeleteAsync(asignacion);
        }

        public async Task<IEnumerable<CandidatoPuestoDto>> GetByPartidoAsync(int partidoId)
        {
            var asignaciones = await _repo.GetByPartidoAsync(partidoId);
            return asignaciones.Select(cp => new CandidatoPuestoDto
            {
                Id = cp.Id,
                CandidatoId = cp.CandidatoId,
                NombreCandidato = cp.Candidato.Nombre,
                ApellidoCandidato = cp.Candidato.Apellido,
                FotoCandidato = cp.Candidato.Foto,
                PuestoElectivoId = cp.PuestoElectivoId,
                NombrePuesto = cp.PuestoElectivo.Nombre,
                PartidoPoliticoId = cp.PartidoPoliticoId,
                NombrePartido = cp.PartidoPolitico.Nombre,
                EsAliado = cp.EsAliado
            });
        }

        public async Task<IEnumerable<CandidatoPuestoDto>> GetCandidatosDisponiblesAsync(int partidoId)
        {
            return await _repo.GetCandidatosDisponiblesAsync(partidoId);
        }

        public async Task<IEnumerable<PuestoElectivoDto>> GetPuestosDisponiblesAsync(int partidoId)
        {
            return await _repo.GetPuestosDisponiblesAsync(partidoId);
        }
    }
}