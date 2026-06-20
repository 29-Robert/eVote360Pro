using eVote360.Core.Application.DTOs.Alianzas;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVote360.Core.Application.Services
{
    public class AlianzaService : IAlianzaService
    {
        
        private readonly IAlianzaRepository _alianzaRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AlianzaService(IAlianzaRepository alianzaRepo,
                              IEleccionRepository eleccionRepo,
                              IHttpContextAccessor httpContextAccessor)
        {
            _alianzaRepo = alianzaRepo;
            _eleccionRepo = eleccionRepo;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task AceptarAsync(int solicitudId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede aceptar una solicitud mientras exista una elección activa.");

            var solicitud = await _alianzaRepo.GetByIdAsync(solicitudId);

           
            var claim = _httpContextAccessor.HttpContext.User.FindFirst("PartidoId");
            if (claim == null)
                throw new Exception("No se pudo identificar al usuario.");

            
            int partidoId = int.Parse(claim.Value);

            if (solicitud.PartidoReceptorId != partidoId)
                throw new Exception("No tiene permisos para responder esta solicitud.");

            if (solicitud.Estado != EstadoSolicitudAlianza.EnEspera)
                throw new Exception("Esta solicitud ya fue respondida.");

            solicitud.Estado = EstadoSolicitudAlianza.Aceptada;
            await _alianzaRepo.UpdateAsync(solicitud);
        }

        public async Task CrearSolicitudAsync(int PartidoSolicitanteId, int PartidoReceptorId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede crear una solicitud mientras exista una elección activa.");

            if (await _alianzaRepo.ExisteAlianzaVigenteAsync(PartidoSolicitanteId, PartidoReceptorId))
                throw new Exception("Ya existe una alianza vigente con este partido político.");

            if (await _alianzaRepo.ExisteSolicitudPendienteAsync(PartidoSolicitanteId, PartidoReceptorId))
                throw new Exception("Ya existe una solicitud de alianza pendiente con este partido.");

            var solicitud = new SolicitudAlianza
            {
                PartidoSolicitanteId = PartidoSolicitanteId,
                PartidoReceptorId = PartidoReceptorId,
                FechaSolicitud = DateTime.Now,
                Estado = EstadoSolicitudAlianza.EnEspera
            };

            await _alianzaRepo.AddAsync(solicitud);
        }
        

        public async Task EliminarAlianzaAsync(int solicitudId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede eliminar una alianza mientras exista una elección activa.");

            var solicitud = await _alianzaRepo.GetByIdAsync(solicitudId);

         
            var claim = _httpContextAccessor.HttpContext.User.FindFirst("PartidoId");
            if (claim == null) throw new Exception("No se pudo identificar al usuario.");

            int partidoId = int.Parse(claim.Value);

            if (solicitud.PartidoSolicitanteId != partidoId && solicitud.PartidoReceptorId != partidoId)
                throw new Exception("No tiene permisos para eliminar esta alianza.");

            if (await _alianzaRepo.TieneCandidatosAliadosAsync(solicitudId))
                throw new Exception("No se puede eliminar esta alianza porque existen candidatos aliados asignados.");

            await _alianzaRepo.DeleteAsync(solicitud);
        }

        public async Task EliminarSolicitudAsync(int solicitudId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede eliminar una solicitud mientras exista una elección activa.");

            var solicitud = await _alianzaRepo.GetByIdAsync(solicitudId);

         
            var claim = _httpContextAccessor.HttpContext.User.FindFirst("PartidoId");
            if (claim == null) throw new Exception("No se pudo identificar al usuario.");

            int partidoIdUsuario = int.Parse(claim.Value); 

            
            if (solicitud.PartidoSolicitanteId != partidoIdUsuario)
                throw new Exception("No tiene permisos para eliminar esta solicitud.");

            if (solicitud.Estado == EstadoSolicitudAlianza.Aceptada)
                throw new Exception("No se puede eliminar una solicitud aceptada porque ya generó una alianza vigente.");

            await _alianzaRepo.DeleteAsync(solicitud);
        }

        public async Task<IEnumerable<SolicitudAlianzaDto>> GetPendientesAsync(int partidoId)
        {
            var solicitudes = await _alianzaRepo.GetPendientesByReceptorAsync(partidoId);
            return solicitudes.Select(s => new SolicitudAlianzaDto
            {
                Id = s.Id,
                NombrePartidoSolicitante = s.PartidoSolicitante.Nombre,
                SiglasPartidoSolicitante = s.PartidoSolicitante.Siglas,
                FechaSolicitud = s.FechaSolicitud,
                Estado = s.Estado
            });
        }

        public async Task<IEnumerable<SolicitudAlianzaDto>> GetRealizadasAsync(int partidoId)
        {
            var solicitudes = await _alianzaRepo.GetRealizadasBySolicitanteAsync(partidoId);
            return solicitudes.Select(s => new SolicitudAlianzaDto
            {
                Id = s.Id,
                PartidoSolicitanteId = s.PartidoSolicitanteId,
                PartidoReceptorId = s.PartidoReceptorId,
                NombrePartidoSolicitante = s.PartidoSolicitante.Nombre,
                SiglasPartidoSolicitante = s.PartidoSolicitante.Siglas,
                NombrePartidoReceptor = s.PartidoReceptor.Nombre,
                SiglasPartidoReceptor = s.PartidoReceptor.Siglas,
                FechaSolicitud = s.FechaSolicitud,
                Estado = s.Estado
            });
        }

        public async Task<IEnumerable<SolicitudAlianzaDto>> GetVigentesAsync(int partidoId)
        {
            var alianzas = await _alianzaRepo.GetVigentesByPartidoAsync(partidoId);
            return alianzas.Select(s => new SolicitudAlianzaDto
            {
                Id = s.Id,
                PartidoSolicitanteId = s.PartidoSolicitanteId,
                PartidoReceptorId = s.PartidoReceptorId,
                NombrePartidoSolicitante = s.PartidoSolicitante.Nombre,
                SiglasPartidoSolicitante = s.PartidoSolicitante.Siglas,
                NombrePartidoReceptor = s.PartidoReceptor.Nombre,
                SiglasPartidoReceptor = s.PartidoReceptor.Siglas,
                FechaSolicitud = s.FechaSolicitud,
                Estado = s.Estado
            });
        }

        public async Task RechazarAsync(int solicitudId)
        {
            if (await _eleccionRepo.ExisteEleccionActiva())
                throw new Exception("No se puede rechazar una solicitud mientras exista una elección activa.");

            var solicitud = await _alianzaRepo.GetByIdAsync(solicitudId);
            var claim = _httpContextAccessor.HttpContext.User.FindFirst("PartidoId");
           
            if (claim == null) throw new Exception("No se pudo identificar al usuario.");

            int partidoId = int.Parse(claim.Value);

            if (solicitud.PartidoReceptorId != partidoId)
                throw new Exception("No tiene permisos para responder esta solicitud.");

            if (solicitud.Estado != EstadoSolicitudAlianza.EnEspera)
                throw new Exception("Esta solicitud ya fue respondida.");

            solicitud.Estado = EstadoSolicitudAlianza.Rechazada;
            await _alianzaRepo.UpdateAsync(solicitud);

        }
    }
}