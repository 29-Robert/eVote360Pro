using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    public class EleccionesController : Controller
    {
        private readonly IEleccionService _eleccionService;
        private readonly ApplicationDbContext _context; 

        public EleccionesController(IEleccionService eleccionService, ApplicationDbContext context)
        {
            _eleccionService = eleccionService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Consultamos directamente la tabla de elecciones en la base de datos SQL Server
                var elecciones = await _context.Elecciones.ToListAsync();
                return View(elecciones);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los procesos electorales: {ex.Message}";
                return View(new System.Collections.Generic.List<Eleccion>());
            }
        }

        // 2. Formulario de Creación (Vista)
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Eleccion { FechaRealizacion = DateTime.Today });
        }

        // 3. Procesar la Creación de la Elección
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Eleccion eleccion)
        {
            if (!ModelState.IsValid)
            {
                return View(eleccion);
            }

            try
            {
                // Forzamos el estado inicial en Pendiente por requerimiento
                eleccion.Estado = EstadoEleccion.Pendiente;

                // Guardamos directamente en la tabla a través de EF Core 10
                await _context.Elecciones.AddAsync(eleccion);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡El proceso electoral ha sido registrado exitosamente en estado Pendiente!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se pudo crear la elección: {ex.Message}";
                return View(eleccion);
            }
        }

        // 4. Transición de Estados usando tu IEleccionService original
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, EstadoEleccion nuevoEstado)
        {
            try
            {
                if (nuevoEstado == EstadoEleccion.Activa)
                {
                    await _eleccionService.ActivarEleccionAsync(id);
                    TempData["SuccessMessage"] = "¡El proceso electoral ha iniciado con éxito! Las votaciones están abiertas.";
                }
                else if (nuevoEstado == EstadoEleccion.Finalizada)
                {
                    await _eleccionService.FinalizarEleccionAsync(id);
                    TempData["SuccessMessage"] = "El proceso electoral ha concluido correctamente. Las votaciones se han cerrado de forma definitiva.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar el estado del proceso: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}