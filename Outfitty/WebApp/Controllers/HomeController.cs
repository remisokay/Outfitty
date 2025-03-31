using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger) // sinna tuleb kliendi käest päring
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // model - anna datat (> - <)/
        // data tuleb nii - View(data)
        return View();
    }
    
    // [HttpPost]  EXAMPLE USAGE
    // public IActionResult Index(int id)
    // {
    //     return View();
    // }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}