using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eVote360.Infrastructure.Persistence.Contexts;
using eVote360.Core.Domain.Enums;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalElections = await _context.Elecciones.CountAsync();
            var totalCitizens = await _context.Ciudadanos.CountAsync();
            var totalParties = await _context.PartidosPoliticos.CountAsync();
            var totalUsers = await _context.Usuarios.CountAsync();

            var activeElection = await _context.Elecciones
                .FirstOrDefaultAsync(e => e.Estado == EstadoEleccion.Activa);

            ViewBag.TotalElections = totalElections;
            ViewBag.TotalCitizens = totalCitizens;
            ViewBag.TotalParties = totalParties;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveElectionName = activeElection != null ? activeElection.Nombre : "Ninguna";
            ViewBag.ActiveElectionStatus = activeElection != null ? "Activa" : "No Activa";

            return View();
        }
    }
}