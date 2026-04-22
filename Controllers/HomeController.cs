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

        var algorithmIdeas = ideas.Select(i => new ComparadorIdeas.Idea(i.Id, i.Text)).ToList();
        var similitudes = ComparadorIdeas.AlgoritmoSimilitud.CompararIdeas(algorithmIdeas);
        
        var ideasConSimilitud = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<IdeasCreativas.Models.Idea>>();
        foreach(var sim in similitudes)
        {
            if (sim.Similitud >= 0.2) // Consideramos "algo parecida" si es >= 20%
            {
                var idea1Original = ideas.First(i => i.Id == sim.Idea1.Id);
                var idea2Original = ideas.First(i => i.Id == sim.Idea2.Id);

                if (!ideasConSimilitud.ContainsKey(sim.Idea1.Id))
                    ideasConSimilitud[sim.Idea1.Id] = new System.Collections.Generic.List<IdeasCreativas.Models.Idea>();
                ideasConSimilitud[sim.Idea1.Id].Add(idea2Original);

                if (!ideasConSimilitud.ContainsKey(sim.Idea2.Id))
                    ideasConSimilitud[sim.Idea2.Id] = new System.Collections.Generic.List<IdeasCreativas.Models.Idea>();
                ideasConSimilitud[sim.Idea2.Id].Add(idea1Original);
            }
        }
        ViewBag.IdeasConSimilitud = ideasConSimilitud;

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
