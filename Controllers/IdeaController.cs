using Microsoft.AspNetCore.Mvc;
using IdeasCreativas.Data;
using IdeasCreativas.Models;
using IdeasCreativas.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdeasCreativas.Controllers
{
    public class IdeaController : Controller
    {
        private readonly AppDbContext _context;

        public IdeaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Postulate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Postulate(IdeaPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name == model.TeamName && t.Password == model.TeamPassword);
                
                if (team == null)
                {
                    ModelState.AddModelError(string.Empty, "Credenciales de equipo incorrectas.");
                    return View(model);
                }

                var newIdea = new Idea
                {
                    Text = model.IdeaText,
                    PostDate = DateTime.Now,
                    IsCreative = false,
                    IsWellFormulated = false,
                    IsApproved = false,
                    TeamId = team.Id
                };

                _context.Ideas.Add(newIdea);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "la idea fue postulada con éxito, espere por aprobación";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
