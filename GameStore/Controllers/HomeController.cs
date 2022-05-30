using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GameStore.Models;

namespace GameStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GamesDBContext _context;

    public HomeController(ILogger<HomeController> logger, GamesDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            User user = _context.GetUserByEmail(User.Identity.Name);
            string? uName = user.Username;
            int? ID = user.Id;
            return RedirectToAction("Index", "GamesCopy", new { username = uName, userId = ID });
        }
        else
            return RedirectToAction("Login", "Account");
    }

    public IActionResult Privacy()
    {
        return View();
    }
}