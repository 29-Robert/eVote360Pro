using Microsoft.AspNetCore.Mvc;

namespace eVote360.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}