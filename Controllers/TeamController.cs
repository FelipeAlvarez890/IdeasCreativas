using Microsoft.AspNetCore.Mvc;
using IdeasCreativas.Data;
using IdeasCreativas.Models;
using IdeasCreativas.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IdeasCreativas.Controllers
{
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;

        public TeamController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(TeamRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verify if team name already exists
                var existingTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Name == model.Name);
                if (existingTeam != null)
                {
                    ModelState.AddModelError("Name", "ya hay un equipo con ese nombre, ingrese otro nombre");
                    return View(model);
                }

                // Validate student uniqueness
                bool isM1Taken = await _context.Teams.AnyAsync(t => t.Member1 == model.Member1 || t.Member2 == model.Member1);
                if (isM1Taken)
                {
                    ModelState.AddModelError("Member1", "Este alumno ya pertenece a otro equipo.");
                    return View(model);
                }

                if (model.MemberCount == 2)
                {
                    if (string.IsNullOrWhiteSpace(model.Member2))
                    {
                        ModelState.AddModelError("Member2", "Debe ingresar el nombre del integrante 2 si el equipo es de 2 personas.");
                        return View(model);
                    }
                    if (model.Member1.Trim().ToLower() == model.Member2.Trim().ToLower())
                    {
                        ModelState.AddModelError("Member2", "Los integrantes no pueden llamarse igual.");
                        return View(model);
                    }
                    bool isM2Taken = await _context.Teams.AnyAsync(t => t.Member1 == model.Member2 || t.Member2 == model.Member2);
                    if (isM2Taken)
                    {
                        ModelState.AddModelError("Member2", "Este alumno ya pertenece a otro equipo.");
                        return View(model);
                    }
                }

                // If Model is valid and name doesn't exist, save it
                var newTeam = new Team
                {
                    Name = model.Name,
                    Password = model.Password,
                    MemberCount = model.MemberCount,
                    Member1 = model.Member1,
                    Member2 = model.MemberCount == 2 ? model.Member2 : null
                };

                _context.Teams.Add(newTeam);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ingresado con exito";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
