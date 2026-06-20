using eVote360.Core.Application.DTOs.Usuarios;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.WebApp.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eVote360.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAsignacionDirigenteService _asignacionService;
        private readonly IPartidoPoliticoService _partidoService;

        public AuthController(
            IUsuarioService usuarioService,
            IAsignacionDirigenteService asignacionService,
            IPartidoPoliticoService partidoService)
        {
            _usuarioService = usuarioService;
            _asignacionService = asignacionService;
            _partidoService = partidoService;
        }

        public IActionResult Index()
        {

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrador")) return RedirectToAction("Dashboard", "Admin");
                if (User.IsInRole("Dirigente político")) return RedirectToAction("Dashboard", "Dirigente");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var loginDto = new LoginDto
            {
                NombreUsuario = vm.NombreUsuario,
                Password = vm.Password
            };

            var user = await _usuarioService.LoginAsync(loginDto);

            if (user == null)
            {
                ModelState.AddModelError("", "Los datos de acceso son inválidos.");
                return View(vm);
            }

            if (!user.Estado)
            {
                ModelState.AddModelError("", "El usuario está inactivo.");
                return View(vm);
            }


            int? partidoPoliticoId = null;
            if (user.Rol == "Dirigente político")
            {
                var asignaciones = await _asignacionService.GetAllAsync();
                var miAsignacion = asignaciones.FirstOrDefault(a => a.UsuarioId == user.Id);

                if (miAsignacion == null)
                {
                    ModelState.AddModelError("", "No tiene un partido político asignado, por lo tanto no puede iniciar sesión. Por favor, póngase en contacto con un administrador.");
                    return View(vm);
                }

                var partidos = await _partidoService.GetAllAsync();
                var miPartido = partidos.FirstOrDefault(p => p.Id == miAsignacion.PartidoPoliticoId);

                if (miPartido == null || !miPartido.Estado)
                {
                    ModelState.AddModelError("", "El partido político asignado a este usuario se encuentra inactivo.");
                    return View(vm);
                }

                partidoPoliticoId = miAsignacion.PartidoPoliticoId;
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.NombreUsuario),
                new Claim(ClaimTypes.Email, user.CorreoElectronico),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            if (partidoPoliticoId.HasValue)
            {
                claims.Add(new Claim("PartidoId", partidoPoliticoId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            HttpContext.Session.SetInt32("UsuarioId", user.Id);


            if (user.Rol == "Administrador")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("Dashboard", "Dirigente");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Auth");
        }
    }
}