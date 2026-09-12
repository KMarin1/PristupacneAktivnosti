using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ProfilController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
