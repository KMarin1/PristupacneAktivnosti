using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Korisnici()
        {
            return View();
        }
    }
}
