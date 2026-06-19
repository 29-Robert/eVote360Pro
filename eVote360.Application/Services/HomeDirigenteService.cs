using eVote360.Core.Application.DTOs.HomeDirigentes;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Services
{
    public class HomeDirigenteService : IHomeDirigenteService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly IAlianzaRepository _alianzaRepo;
        private readonly ICandidatoPuestoRepository _asignacionRepo;

        public HomeDirigenteService(
            IUsuarioRepository usuarioRepo,
            ICandidatoRepository candidatoRepo,
            IAlianzaRepository alianzaRepo,
            ICandidatoPuestoRepository asignacionRepo)
        {
            _usuarioRepo = usuarioRepo;
            _candidatoRepo = candidatoRepo;
            _alianzaRepo = alianzaRepo;
            _asignacionRepo = asignacionRepo;
        }

        public async Task<(bool EsValido, string MensajeError, int? PartidoId)> ValidarAccesoAsync(int usuarioId)
        {
            var usuario = await _usuarioRepo.GetByIdWithPartidoAsync(usuarioId);

            if (usuario == null)
                return (false, "Usuario no encontrado.", null);

            // CORRECCIÓN: Validamos pasando por el puente "AsignacionDirigente"
            if (usuario.AsignacionDirigente == null || usuario.AsignacionDirigente.PartidoPoliticoId == null)
                return (false, "No tiene un partido político asignado. Por favor, póngase en contacto con un administrador.", null);

            // CORRECCIÓN: Extraemos el partido desde la asignación
            var partido = usuario.AsignacionDirigente.PartidoPolitico;

            if (partido == null || !partido.Estado)
                return (false, "El partido político asignado a este usuario se encuentra inactivo.", null);

            // Retornamos el ID del partido correctamente
            return (true, string.Empty, partido.Id);
        }

        public async Task<HomeDirigentesDto> GetIndicadoresAsync(int partidoId)
        {
            var usuario = await _usuarioRepo.GetByPartidoIdAsync(partidoId);

           
            var partido = usuario?.AsignacionDirigente?.PartidoPolitico;

            return new HomeDirigentesDto
            {
                
                NombrePartido = partido?.Nombre ?? string.Empty,
                Siglas = partido?.Siglas ?? string.Empty,
                Logo = partido?.LogoUrl ?? string.Empty,
                CandidatosActivos = await _candidatoRepo.CountActivosAsync(partidoId),
                CandidatosInactivos = await _candidatoRepo.CountInactivosAsync(partidoId),
                AlianzasVigentes = await _alianzaRepo.CountVigentesAsync(partidoId),
                SolicitudesPendientes = await _alianzaRepo.CountPendientesAsync(partidoId),
                CandidatosAsignados = await _asignacionRepo.CountByPartidoAsync(partidoId)
            };
        }
    }
}