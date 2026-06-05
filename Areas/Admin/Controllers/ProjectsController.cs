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
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User)!);
        }

        // GET: Admin/Projects
        public async Task<IActionResult> Index(string searchString)
        {
            var userId = GetCurrentUserId();

            var projectsQuery = _context.Projects
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                projectsQuery = projectsQuery.Where(p => p.Title.Contains(searchString) || p.Description.Contains(searchString));
            }

            var projects = await projectsQuery
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            ViewData["CurrentFilter"] = searchString;

            return View(projects);
        }

        // GET: Admin/Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var project = await _context.Projects
                .Include(p => p.ProjectSkills)
                    .ThenInclude(ps => ps.Skill)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (project == null) return NotFound();

            return View(project);
        }

        // GET: Admin/Projects/Create
        public async Task<IActionResult> Create()
        {
            var userId = GetCurrentUserId();
            ViewBag.Skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            ViewBag.SelectedSkillIds = new List<int>();
            return View();
        }

        // POST: Admin/Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,RepositoryUrl,LiveDemoUrl,ImageUrl,IsFeatured,DisplayOrder,CompletionDate")] Project project, int[] selectedSkillIds)
        {
            var userId = GetCurrentUserId();
            project.ApplicationUserId = userId;

            // Clear model state error for ApplicationUser since it's populated manually
            ModelState.Remove("ApplicationUser");

            // Uniqueness check for DisplayOrder
            if (await _context.Projects.AnyAsync(p => p.ApplicationUserId == userId && p.DisplayOrder == project.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "This Display Order is already in use by another project.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();

                if (selectedSkillIds != null)
                {
                    foreach (var skillId in selectedSkillIds)
                    {
                        _context.ProjectSkills.Add(new ProjectSkill
                        {
                            ProjectId = project.Id,
                            SkillId = skillId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            ViewBag.SelectedSkillIds = selectedSkillIds?.ToList() ?? new List<int>();
            return View(project);
        }

        // GET: Admin/Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == userId);

            if (project == null) return NotFound();

            ViewBag.Skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            ViewBag.SelectedSkillIds = await _context.ProjectSkills
                .AsNoTracking()
                .Where(ps => ps.ProjectId == project.Id)
                .Select(ps => ps.SkillId)
                .ToListAsync();

            return View(project);
        }

        // POST: Admin/Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,RepositoryUrl,LiveDemoUrl,ImageUrl,IsFeatured,DisplayOrder,CompletionDate")] Project project, int[] selectedSkillIds)
        {
            if (id != project.Id) return NotFound();

            var userId = GetCurrentUserId();
            project.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            // Uniqueness check for DisplayOrder
            if (await _context.Projects.AnyAsync(p => p.ApplicationUserId == userId && p.DisplayOrder == project.DisplayOrder && p.Id != project.Id))
            {
                ModelState.AddModelError("DisplayOrder", "This Display Order is already in use by another project.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();

                    // Update ProjectSkills
                    var existingProjectSkills = await _context.ProjectSkills
                        .Where(ps => ps.ProjectId == project.Id)
                        .ToListAsync();
                    _context.ProjectSkills.RemoveRange(existingProjectSkills);

                    if (selectedSkillIds != null)
                    {
                        foreach (var skillId in selectedSkillIds)
                        {
                            _context.ProjectSkills.Add(new ProjectSkill
                            {
                                ProjectId = project.Id,
                                SkillId = skillId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            ViewBag.SelectedSkillIds = selectedSkillIds?.ToList() ?? new List<int>();
            return View(project);
        }

        // GET: Admin/Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (project == null) return NotFound();

            return View(project);
        }

        // POST: Admin/Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == userId);

            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id, int userId)
        {
            return _context.Projects.Any(e => e.Id == id && e.ApplicationUserId == userId);
        }
    }
}