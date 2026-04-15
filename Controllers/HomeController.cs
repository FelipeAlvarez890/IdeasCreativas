using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdeasCreativas.Models;
using IdeasCreativas.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdeasCreativas.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ideas = await _context.Ideas
            .Include(i => i.Team)
            .OrderByDescending(i => i.PostDate)
            .ToListAsync();
        return View(ideas);
    }

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
