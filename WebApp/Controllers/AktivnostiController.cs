using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AktivnostiController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Add() => View();

        public IActionResult Edit() => View();

        public IActionResult Delete() => View();
        public IActionResult Details() => View();

    }
}
