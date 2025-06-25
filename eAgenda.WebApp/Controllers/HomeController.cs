using eAgenda.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace eAgenda.WebApp.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("erro")]
    public IActionResult Erro()
    {
        return View();
    }
}
