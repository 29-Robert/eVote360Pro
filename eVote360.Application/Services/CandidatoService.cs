using eVote360.Core.Application.DTOs.Candidato;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly IEleccionRepository _eleccionRepo;

        public CandidatoService(ICandidatoRepository candidatoRepo, IEleccionRepository eleccionRepo)
        {
            _candidatoRepo = candidatoRepo;
            _eleccionRepo = eleccionRepo;
        }

        private async Task ValidarEleccionActiva()
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se pueden realizar cambios mientras exista una elección activa.");
        }

        public async Task AddAsync(SaveCandidatoDto dto)
        {
            await ValidarEleccionActiva();

            var entity = new Candidato
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DocumentoIdentidad = System.Text.RegularExpressions.Regex.Replace(dto.DocumentoIdentidad, @"[^\d]", ""),
                CorreoElectronico = dto.CorreoElectronico,
                PartidoPoliticoId = dto.PartidoPoliticoId,
                PuestoElectivoId = dto.PuestoElectivoId,
                Foto = dto.Foto ?? "/images/default.png",
                EsActivo = true
            };
            await _candidatoRepo.AddAsync(entity);
        }

        public async Task UpdateAsync(SaveCandidatoDto dto)
        {
            await ValidarEleccionActiva();

            var entity = await _candidatoRepo.GetByIdAsync(dto.Id);
            entity.Nombre = dto.Nombre;
            entity.Apellido = dto.Apellido;
            entity.DocumentoIdentidad = System.Text.RegularExpressions.Regex.Replace(dto.DocumentoIdentidad, @"[^\d]", "");
            entity.CorreoElectronico = dto.CorreoElectronico;
            entity.PartidoPoliticoId = dto.PartidoPoliticoId;
            entity.PuestoElectivoId = dto.PuestoElectivoId;

            if (!string.IsNullOrEmpty(dto.Foto))
                entity.Foto = dto.Foto;

            await _candidatoRepo.UpdateAsync(entity);
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
                PuestoAsignado = c.PuestoElectivo?.Nombre ?? "Sin puesto asociado",
                DocumentoIdentidad = c.DocumentoIdentidad,
                CorreoElectronico = c.CorreoElectronico,
                PartidoPoliticoId = c.PartidoPoliticoId,
                PuestoElectivoId = c.PuestoElectivoId
            };
        }

        public async Task<IEnumerable<CandidatoDTO>> GetByPartidoAsync(int partidoId)
        {
            var list = await _candidatoRepo.GetByPartidoAsync(partidoId);
            return list.Select(c => new CandidatoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Foto = c.Foto,
                EsActivo = c.EsActivo,
                NombrePartido = c.PartidoPolitico.Nombre
            });
        }

        public async Task ChangeStateAsync(int id, bool estado)
        {
            await ValidarEleccionActiva();
            var candidato = await _candidatoRepo.GetByIdAsync(id);
            candidato.EsActivo = estado;
            await _candidatoRepo.UpdateAsync(candidato);
        }
    }
}