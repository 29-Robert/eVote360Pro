using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Enums;
using eVote360.Infrastructure.Persistence.Contexts;
using eVote360.WebApp.ViewModels.HomeDirigente;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVote360.WebApp.Controllers
{
    public class DirigenteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DirigenteController(ApplicationDbContext context)
        {
            _context = context;
        }

     
        private int? GetUsuarioIdSesion()
        {
            return HttpContext.Session.GetInt32("UsuarioId");
        }

       
        public async Task<IActionResult> Index()
        {
            
            var usuarioId = GetUsuarioIdSesion();
            if (usuarioId is null)
                return RedirectToAction("Login", "Account");

          
            var usuario = await _context.Usuarios
                .Include(u => u.AsignacionDirigente)
                    .ThenInclude(a => a!.PartidoPolitico)
                .FirstOrDefaultAsync(u => u.Id == usuarioId && u.Estado);

            if (usuario is null)
                return RedirectToAction("Login", "Account");

       
            if (usuario.Rol != "DirigentePolitico")
                return RedirectToAction("AccesoDenegado", "Account");

         
            var asignacion = usuario.AsignacionDirigente;
            if (asignacion is null || asignacion.PartidoPolitico is null)
            {
                TempData["Error"] = "No tiene un partido político asignado. Contacte al administrador.";
                return RedirectToAction("Login", "Account");
            }

            var partido = asignacion.PartidoPolitico;

            if (!partido.Estado)
            {
                TempData["Error"] = "Su partido político se encuentra inactivo. Contacte al administrador.";
                return RedirectToAction("Login", "Account");
            }

   
            var totalCandidatos = await _context.Candidatos
                .CountAsync(c => c.PartidoPoliticoId == partido.Id);

            var candidatosActivos = await _context.Candidatos
                .CountAsync(c => c.PartidoPoliticoId == partido.Id && c.EsActivo);

            var candidatosAsignados = await _context.CandidatosPuestos
                .CountAsync(a => a.Candidato!.PartidoPoliticoId == partido.Id);

            var alianzasPendientes = await _context.SolicitudesAlianzas
           .CountAsync(a =>
         (a.PartidoSolicitanteId == partido.Id || a.PartidoReceptorId == partido.Id)
         && a.Estado == EstadoSolicitudAlianza.EnEspera);

            var alianzasAceptadas = await _context.SolicitudesAlianzas
                .CountAsync(a =>
                    (a.PartidoSolicitanteId == partido.Id || a.PartidoReceptorId == partido.Id)
                    && a.Estado == EstadoSolicitudAlianza.Aceptada);

            var vm = new HomeDirigenteViewModel
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                NombreUsuario = usuario.NombreUsuario,
                PartidoPoliticoId = partido.Id,
                NombrePartido = partido.Nombre,
                SiglasPartido = partido.Siglas,
                PartidoActivo = partido.Estado,
                TotalCandidatos = totalCandidatos,
                CandidatosActivos = candidatosActivos,
                CandidatosAsignados = candidatosAsignados,
                AlianzasPendientes = alianzasPendientes,
                AlianzasAceptadas = alianzasAceptadas,
            };

            return View(vm);
        }
    }

}
