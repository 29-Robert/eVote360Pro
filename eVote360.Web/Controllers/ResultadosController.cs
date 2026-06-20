using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eVote360.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ResultadosController : Controller
    {
        private readonly IResultadoService _resultadoService;

        public ResultadosController(IResultadoService resultadoService)
        {
            _resultadoService = resultadoService;
        }

        [HttpGet]
        public async Task<IActionResult> Escrutinio(int eleccionId)
        {
            try
            {
                var resultados = await _resultadoService.CalcularResultadosAsync(eleccionId);
                return View(resultados);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Elecciones");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Elecciones");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al procesar los resultados.";
                return RedirectToAction("Index", "Elecciones");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Resumen(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            ViewBag.SelectedYear = selectedYear;

            try
            {
                var resumenes = await _resultadoService.GetResumenAnualAsync(selectedYear);
                return View(resumenes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al obtener el resumen electoral: {ex.Message}";
                return View(new List<eVote360.Core.Application.DTOs.Resultado.ResumenEleccionAnualDto>());
            }
        }
    }
}