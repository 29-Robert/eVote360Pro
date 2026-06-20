using eVote360.WebApp.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            string controladorDestino = "Elector"; 
            string accionDestino = "Index";
            string textoBoton = "Ir al inicio";

            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrador"))
                {
                    controladorDestino = "Admin";
                    accionDestino = "Dashboard";
                    textoBoton = "Panel Administrativo";
                }
                else if (User.IsInRole("Dirigente político"))
                {
                    controladorDestino = "Dirigente";
                    accionDestino = "Dashboard";
                    textoBoton = "Mi Panel de Dirigente";
                }
            }

            var viewModel = new AccesoDenegadoViewModel
            {
                Titulo = "Acceso Denegado",
                Mensaje = "No tienes permisos para acceder a esta sección del sistema.",
                ControladorDestino = controladorDestino,
                AccionDestino = accionDestino,
                TextoBoton = textoBoton
            };

            return View(viewModel);
        }
    }
}