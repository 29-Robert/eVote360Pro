using eVote360.Core.Application.DTOs.Ciudadanos;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Services
{
    public class CiudadanoService : ICiudadanoService
    {
        private readonly IGenericRepository<Ciudadano> _repository;
        private readonly IEleccionValidationService _eleccionValidationService;
        private readonly IVotoRepository _votoRepository;

        public CiudadanoService(
            IGenericRepository<Ciudadano> repository,
            IEleccionValidationService eleccionValidationService,
            IVotoRepository votoRepository)
        {
            _repository = repository;
            _eleccionValidationService = eleccionValidationService;
            _votoRepository = votoRepository;
        }

        public async Task<SaveCiudadanoDto> AddAsync(SaveCiudadanoDto dto)
        {
            await EnsureNoEleccionActivaAsync(
                "No se puede crear un ciudadano mientras exista una elección activa.");

            var documentoNormalizado = NormalizarDocumentoIdentidad(dto.DocumentoIdentidad);
            var correoNormalizado = NormalizarCorreo(dto.CorreoElectronico);

            await ValidarUnicidadAsync(documentoNormalizado, correoNormalizado);

            var entity = new Ciudadano
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                CorreoElectronico = correoNormalizado,
                DocumentoIdentidad = documentoNormalizado,
                Estado = dto.Estado
            };

            var addedEntity = await _repository.AddAsync(entity);
            dto.Id = addedEntity.Id;
            dto.DocumentoIdentidad = documentoNormalizado;
            dto.CorreoElectronico = correoNormalizado;
            dto.PuedeEditarDocumentoIdentidad = true;
            return dto;
        }

        public async Task UpdateAsync(SaveCiudadanoDto dto)
        {
            await EnsureNoEleccionActivaAsync(
                "No se puede editar un ciudadano mientras exista una elección activa.");

            var entity = await _repository.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"El ciudadano con ID {dto.Id} no existe.");

            var documentoNormalizado = NormalizarDocumentoIdentidad(dto.DocumentoIdentidad);
            var correoNormalizado = NormalizarCorreo(dto.CorreoElectronico);

            await ValidarUnicidadAsync(documentoNormalizado, correoNormalizado, dto.Id);

            if (documentoNormalizado != entity.DocumentoIdentidad
                && !await PuedeModificarDocumentoIdentidadAsync(dto.Id))
            {
                throw new InvalidOperationException(
                    "No se puede modificar el número de documento de identidad de este ciudadano porque ya participó en una elección.");
            }

            entity.Nombre = dto.Nombre.Trim();
            entity.Apellido = dto.Apellido.Trim();
            entity.CorreoElectronico = correoNormalizado;
            entity.DocumentoIdentidad = documentoNormalizado;
            entity.Estado = dto.Estado;

            await _repository.UpdateAsync(entity);
        }

        public async Task<List<CiudadanoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(e => new CiudadanoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                CorreoElectronico = e.CorreoElectronico,
                DocumentoIdentidad = e.DocumentoIdentidad,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<SaveCiudadanoDto?> GetByIdSaveDtoAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SaveCiudadanoDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                CorreoElectronico = entity.CorreoElectronico,
                DocumentoIdentidad = entity.DocumentoIdentidad,
                Estado = entity.Estado,
                PuedeEditarDocumentoIdentidad = await PuedeModificarDocumentoIdentidadAsync(id)
            };
        }

        public async Task ChangeStateAsync(int id, bool state)
        {
            await EnsureNoEleccionActivaAsync(state
                ? "No se puede activar un ciudadano mientras exista una elección activa."
                : "No se puede desactivar un ciudadano mientras exista una elección activa.");

            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"El ciudadano con ID {id} no existe.");

            if (state && entity.Estado)
            {
                throw new InvalidOperationException("Este ciudadano ya se encuentra activo.");
            }

            if (!state && !entity.Estado)
            {
                throw new InvalidOperationException("Este ciudadano ya se encuentra inactivo.");
            }

            entity.Estado = state;
            await _repository.UpdateAsync(entity);
        }

        public async Task<bool> PuedeModificarDocumentoIdentidadAsync(int ciudadanoId)
        {
            return !await _votoRepository.HaParticipadoEnEleccionActivaOFinalizadaAsync(ciudadanoId);
        }

        private async Task EnsureNoEleccionActivaAsync(string mensaje)
        {
            if (await _eleccionValidationService.HasEleccionActivaAsync())
            {
                throw new InvalidOperationException(mensaje);
            }
        }

        private async Task ValidarUnicidadAsync(string documentoIdentidad, string correoElectronico, int? excludeId = null)
        {
            var ciudadanos = await _repository.GetAllAsync();

            if (ciudadanos.Any(c => c.DocumentoIdentidad == documentoIdentidad && c.Id != excludeId))
            {
                throw new InvalidOperationException(
                    "Ya existe un ciudadano registrado con este número de documento de identidad.");
            }

            if (ciudadanos.Any(c => c.CorreoElectronico == correoElectronico && c.Id != excludeId))
            {
                throw new InvalidOperationException(
                    "Ya existe un ciudadano registrado con este correo electrónico.");
            }
        }

        private static string NormalizarDocumentoIdentidad(string documentoIdentidad)
        {
            return documentoIdentidad.Trim().Replace("-", string.Empty);
        }

        private static string NormalizarCorreo(string correoElectronico)
        {
            return correoElectronico.Trim();
        }
    }
}