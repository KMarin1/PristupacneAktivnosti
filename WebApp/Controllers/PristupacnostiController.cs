using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class PristupacnostiController : Controller
    {
        [HttpGet] public IActionResult Index() => View();
        [HttpGet] public IActionResult Add() => View();
        [HttpGet] public IActionResult Edit() => View();
        [HttpGet("{id:int}")] public IActionResult Edit(int id) { ViewBag.Id = id; return View(); }
    }
}
