using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Admin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<WebApp.Controllers.HomeController> _logger;

    public HomeController(ILogger<WebApp.Controllers.HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}