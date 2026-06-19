using eVote360.Core.Application.DTOs.Votaciones;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    public class EleccionesController : Controller
    {
        private readonly IEleccionService _eleccionService;

        // 🛠️ Corrección: Eliminamos la inyección directa del DbContext para respetar Onion Architecture
        public EleccionesController(IEleccionService eleccionService)
        {
            _eleccionService = eleccionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // 🛠️ Usamos el servicio de aplicación en lugar de consultar la BD directamente
                var elecciones = await _eleccionService.GetAllAsync();
                return View(elecciones);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los procesos electorales: {ex.Message}";
                return View(new System.Collections.Generic.List<Eleccion>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new SaveEleccionDto { FechaRealizacion = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SaveEleccionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                // 🛠️ Delegamos la creación al servicio de negocio
                await _eleccionService.CrearEleccionAsync(dto);

                TempData["SuccessMessage"] = "¡El proceso electoral ha sido registrado exitosamente en estado Pendiente!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se pudo crear la elección: {ex.Message}";
                return View(dto);
            }
        }

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