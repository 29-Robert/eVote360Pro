using eVote360.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    public class DirigenteController : Controller
    {
        private readonly IHomeDirigenteService _homeService;

        public DirigenteController(IHomeDirigenteService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var partidoId = int.Parse(User.FindFirst("PartidoId").Value);
            var dto = await _homeService.GetIndicadoresAsync(partidoId);
            return View(dto);
        }
    }

}
