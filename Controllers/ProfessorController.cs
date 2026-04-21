using Microsoft.AspNetCore.Mvc;
using IdeasCreativas.Data;
using IdeasCreativas.Models;
using IdeasCreativas.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ComparadorIdeas;

namespace IdeasCreativas.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly AppDbContext _context;

        public ProfessorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(ProfessorLoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (model.User == "profe" && model.Password == "123")
                {
                    HttpContext.Session.SetString("IsAdmin", "true");
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("ValidateList");
                }
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ValidateList()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", new { returnUrl = Request.Path + Request.QueryString });
            }

            var teams = await _context.Teams
                .Include(t => t.Ideas)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(teams);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewIdea(int id, string? returnUrl = null)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", new { returnUrl = Request.Path + Request.QueryString });
            }

            var idea = await _context.Ideas.Include(i => i.Team).FirstOrDefaultAsync(i => i.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(idea);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewIdea(int id, bool IsCreative, bool IsWellFormulated, bool IsApproved, bool IsRejected, string? RejectionReason, string? returnUrl = null)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", new { returnUrl = Request.Path + Request.QueryString });
            }

            var idea = await _context.Ideas.Include(i => i.Team).FirstOrDefaultAsync(i => i.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            if (IsApproved && (!IsCreative || !IsWellFormulated))
            {
                ModelState.AddModelError(string.Empty, "La idea tiene que ser creativa y bien planteada para poder ser aprobada.");
                return View(idea);
            }

            if (IsApproved && IsRejected)
            {
                ModelState.AddModelError(string.Empty, "Una idea no puede estar aprobada y rechazada al mismo tiempo.");
                return View(idea);
            }

            if (IsRejected && string.IsNullOrWhiteSpace(RejectionReason))
            {
                ModelState.AddModelError(string.Empty, "Debe especificar el motivo por el cual la idea es rechazada.");
                return View(idea);
            }

            // Check max 2 approved logic
            if (IsApproved && !idea.IsApproved)
            {
                var currentApprovedCount = await _context.Ideas.CountAsync(i => i.TeamId == idea.TeamId && i.IsApproved && i.Id != idea.Id);
                if (currentApprovedCount >= 2)
                {
                    ModelState.AddModelError(string.Empty, "Este equipo ya tiene el máximo de 2 ideas aprobadas.");
                    return View(idea);
                }
            }

            idea.IsCreative = IsCreative;
            idea.IsWellFormulated = IsWellFormulated;
            idea.IsApproved = IsApproved;
            idea.IsRejected = IsRejected;
            idea.RejectionReason = IsRejected ? RejectionReason : null;

            await _context.SaveChangesAsync();

            if (IsCreative && IsWellFormulated && IsApproved)
            {
                var approvedCount = await _context.Ideas.CountAsync(i => i.TeamId == idea.TeamId && i.IsApproved);
                
                if (approvedCount >= 2)
                {
                    var pendingIdeas = await _context.Ideas
                        .Where(i => i.TeamId == idea.TeamId && !i.IsApproved)
                        .ToListAsync();

                    if (pendingIdeas.Any())
                    {
                        _context.Ideas.RemoveRange(pendingIdeas);
                        await _context.SaveChangesAsync();
                    }
                    TempData["SuccessMessage"] = "Este equipo ya tiene 2 ideas aprobadas";
                }
                else
                {
                    TempData["SuccessMessage"] = "idea aprobada";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("ValidateList");
        }

        [HttpGet]
        public async Task<IActionResult> IdeaSimilarity()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login", new { returnUrl = Request.Path + Request.QueryString });
            }

            var dbIdeas = await _context.Ideas.Include(i => i.Team).ToListAsync();
            var algorithmIdeas = dbIdeas.Select(i => new ComparadorIdeas.Idea(i.Id, i.Text)).ToList();

            var similitudes = ComparadorIdeas.AlgoritmoSimilitud.CompararIdeas(algorithmIdeas);

            // Fetch teams to show in UI
            ViewBag.OriginalIdeas = dbIdeas;

            return View(similitudes);
        }
    }
}
