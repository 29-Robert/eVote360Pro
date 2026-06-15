using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVote360.Core.Application.DTOs;

namespace eVote360.Core.Application.Services
{
    public class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly IEleccionRepository _eleccionRepo;

        public CandidatoService(ICandidatoRepository candidatoRepo,
                                IEleccionRepository eleccionRepo)
        {
            _candidatoRepo = candidatoRepo;
            _eleccionRepo = eleccionRepo;
        }

        public async Task ActivarAsync(int id)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede activar un candidato mientras exista una elección activa.");

            var candidato = await _candidatoRepo.GetByIdAsync(id);
            candidato.EsActivo = true;
            await _candidatoRepo.UpdateAsync(candidato);
        }
        

        public async Task CreateAsync(CrearCandidatoDto dto)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede crear un candidato mientras exista una elección activa.");

            var candidato = new Candidato
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Foto = dto.Foto,
                EsActivo = true,
                PartidoPoliticoId = dto.PartidoPoliticoId
            };

            await _candidatoRepo.AddAsync(candidato);
        }

        public async Task DesactivarAsync(int id)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede desactivar un candidato mientras exista una elección activa.");

            var candidato = await _candidatoRepo.GetByIdAsync(id);

            if (candidato.PuestoElectivoId != null)
                throw new Exception("No se puede desactivar este candidato porque está asignado a un puesto electivo.");

            candidato.EsActivo = false;
            await _candidatoRepo.UpdateAsync(candidato);
        }

        public async Task<CandidatoDTO> GetByIdAsync(int id)
        {
            var c = await _candidatoRepo.GetByIdAsync(id);
            return new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Foto = c.Foto,
                EsActivo = c.EsActivo,
                NombrePartido = c.PartidoPolitico.Nombre,
                PuestoAsignado = c.PuestoElectivo?.Nombre ?? "Sin puesto asociado"
            };
        }

        public async Task<IEnumerable<CandidatoDTO>> GetByPartidoAsync(int partidoId)
        {
            var candidatos = await _candidatoRepo.GetByPartidoAsync(partidoId);
            return candidatos.Select(c => new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Foto = c.Foto,
                EsActivo = c.EsActivo,
                NombrePartido = c.PartidoPolitico.Nombre,
                PuestoAsignado = c.PuestoElectivo?.Nombre ?? "Sin puesto asociado"
            });
        }
        

        public async Task UpdateAsync(CandidatoUpdateDto dto)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede editar un candidato mientras exista una elección activa.");

            var candidato = await _candidatoRepo.GetByIdAsync(dto.Id);

            candidato.Nombre = dto.Nombre;
            candidato.Apellido = dto.Apellido;
            candidato.EsActivo = dto.EsActivo;

            if (dto.Foto != null)
                candidato.Foto = dto.Foto;

            await _candidatoRepo.UpdateAsync(candidato);
        }
    }
    

}
