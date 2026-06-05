using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

namespace Portfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SkillsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User)!);
        }

        // GET: Admin/Skills
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            return View(skills);
        }

        // GET: Admin/Skills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var skill = await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (skill == null) return NotFound();

            return View(skill);
        }

        // GET: Admin/Skills/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Skills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,ProficiencyLevel,IconClass,DisplayOrder")] Skill skill)
        {
            var userId = GetCurrentUserId();
            skill.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            // Uniqueness check for DisplayOrder
            if (await _context.Skills.AnyAsync(s => s.ApplicationUserId == userId && s.DisplayOrder == skill.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "This Display Order is already in use by another skill.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Admin/Skills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var skill = await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == userId);

            if (skill == null) return NotFound();

            return View(skill);
        }

        // POST: Admin/Skills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,ProficiencyLevel,IconClass,DisplayOrder")] Skill skill)
        {
            if (id != skill.Id) return NotFound();

            var userId = GetCurrentUserId();
            skill.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            // Uniqueness check for DisplayOrder
            if (await _context.Skills.AnyAsync(s => s.ApplicationUserId == userId && s.DisplayOrder == skill.DisplayOrder && s.Id != skill.Id))
            {
                ModelState.AddModelError("DisplayOrder", "This Display Order is already in use by another skill.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Admin/Skills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var skill = await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (skill == null) return NotFound();

            return View(skill);
        }

        // POST: Admin/Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == userId);

            if (skill != null)
            {
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(int id, int userId)
        {
            return _context.Skills.Any(e => e.Id == id && e.ApplicationUserId == userId);
        }
    }
}
