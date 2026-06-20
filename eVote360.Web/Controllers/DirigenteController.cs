using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Enums;
using eVote360.Infrastructure.Persistence.Contexts;
using eVote360.WebApp.ViewModels.HomeDirigente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Dirigente político")]
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

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Dashboard()
        {
            var usuarioId = GetUsuarioIdSesion();
            if (usuarioId is null)
                return RedirectToAction("Index", "Auth");

            var usuario = await _context.Usuarios
                .Include(u => u.AsignacionDirigente)
                    .ThenInclude(a => a!.PartidoPolitico)
                .FirstOrDefaultAsync(u => u.Id == usuarioId && u.Estado);

            if (usuario is null)
                return RedirectToAction("Index", "Auth");

            if (usuario.Rol != "Dirigente político")
                return RedirectToAction("AccesoDenegado", "Home");

            var asignacion = usuario.AsignacionDirigente;
            if (asignacion is null || asignacion.PartidoPolitico is null)
            {
                TempData["ErrorMessage"] = "No tiene un partido político asignado. Contacte al administrador.";
                return RedirectToAction("Index", "Auth");
            }

            var partido = asignacion.PartidoPolitico;

            if (!partido.Estado)
            {
                TempData["ErrorMessage"] = "Su partido político se encuentra inactivo. Contacte al administrador.";
                return RedirectToAction("Index", "Auth");
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

            var vm = new HomeDirigenteViewModel
            {
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                NombrePartido = partido.Nombre,
                SiglasPartido = partido.Siglas,
                PartidoActivo = partido.Estado,
                TotalCandidatos = totalCandidatos,
                CandidatosActivos = candidatosActivos,
                CandidatosAsignados = candidatosAsignados,
                AlianzasPendientes = alianzasPendientes
            };

            return View("Dashboard", vm);
        }

        public async Task<IActionResult> MiPartido()
        {
            var usuarioId = GetUsuarioIdSesion();
            if (usuarioId is null)
                return RedirectToAction("Index", "Auth");

            var usuario = await _context.Usuarios
                .Include(u => u.AsignacionDirigente)
                    .ThenInclude(a => a!.PartidoPolitico)
                .FirstOrDefaultAsync(u => u.Id == usuarioId && u.Estado);

            if (usuario is null || usuario.Rol != "Dirigente político")
                return RedirectToAction("AccesoDenegado", "Home");

            var asignacion = usuario.AsignacionDirigente;
            if (asignacion is null || asignacion.PartidoPolitico is null)
            {
                TempData["ErrorMessage"] = "No tiene un partido político asignado. Contacte al administrador.";
                return RedirectToAction("Index", "Auth");
            }

            var partido = asignacion.PartidoPolitico;


            var candidatos = await _context.Candidatos
                .Include(c => c.PuestoElectivo)
                .Where(c => c.PartidoPoliticoId == partido.Id)
                .ToListAsync();

            ViewBag.Candidatos = candidatos;

            return View(partido);
        }
    }

}