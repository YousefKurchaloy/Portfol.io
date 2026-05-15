using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

namespace Portfol.io.Controllers
{
    public class ProjectSkillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectSkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProjectSkills
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProjectSkills.Include(p => p.Project).Include(p => p.Skill);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProjectSkills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectSkill = await _context.ProjectSkills
                .Include(p => p.Project)
                .Include(p => p.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectSkill == null)
            {
                return NotFound();
            }

            return View(projectSkill);
        }

        // GET: ProjectSkills/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description");
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name");
            return View();
        }

        // POST: ProjectSkills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VersionUsed,ProjectId,SkillId")] ProjectSkill projectSkill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectSkill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectSkill.ProjectId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", projectSkill.SkillId);
            return View(projectSkill);
        }

        // GET: ProjectSkills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectSkill = await _context.ProjectSkills.FindAsync(id);
            if (projectSkill == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectSkill.ProjectId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", projectSkill.SkillId);
            return View(projectSkill);
        }

        // POST: ProjectSkills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VersionUsed,ProjectId,SkillId")] ProjectSkill projectSkill)
        {
            if (id != projectSkill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectSkill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectSkillExists(projectSkill.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectSkill.ProjectId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", projectSkill.SkillId);
            return View(projectSkill);
        }

        // GET: ProjectSkills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectSkill = await _context.ProjectSkills
                .Include(p => p.Project)
                .Include(p => p.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectSkill == null)
            {
                return NotFound();
            }

            return View(projectSkill);
        }

        // POST: ProjectSkills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectSkill = await _context.ProjectSkills.FindAsync(id);
            if (projectSkill != null)
            {
                _context.ProjectSkills.Remove(projectSkill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectSkillExists(int id)
        {
            return _context.ProjectSkills.Any(e => e.Id == id);
        }
    }
}
