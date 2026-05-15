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
    public class CredentialSkillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CredentialSkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CredentialSkills
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CredentialSkills.Include(c => c.Credential).Include(c => c.Skill);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CredentialSkills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credentialSkill = await _context.CredentialSkills
                .Include(c => c.Credential)
                .Include(c => c.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credentialSkill == null)
            {
                return NotFound();
            }

            return View(credentialSkill);
        }

        // GET: CredentialSkills/Create
        public IActionResult Create()
        {
            ViewData["CredentialId"] = new SelectList(_context.Credentials, "Id", "IssuingAuthority");
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name");
            return View();
        }

        // POST: CredentialSkills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsCoreFocus,CredentialId,SkillId")] CredentialSkill credentialSkill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(credentialSkill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CredentialId"] = new SelectList(_context.Credentials, "Id", "IssuingAuthority", credentialSkill.CredentialId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", credentialSkill.SkillId);
            return View(credentialSkill);
        }

        // GET: CredentialSkills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credentialSkill = await _context.CredentialSkills.FindAsync(id);
            if (credentialSkill == null)
            {
                return NotFound();
            }
            ViewData["CredentialId"] = new SelectList(_context.Credentials, "Id", "IssuingAuthority", credentialSkill.CredentialId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", credentialSkill.SkillId);
            return View(credentialSkill);
        }

        // POST: CredentialSkills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsCoreFocus,CredentialId,SkillId")] CredentialSkill credentialSkill)
        {
            if (id != credentialSkill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credentialSkill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialSkillExists(credentialSkill.Id))
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
            ViewData["CredentialId"] = new SelectList(_context.Credentials, "Id", "IssuingAuthority", credentialSkill.CredentialId);
            ViewData["SkillId"] = new SelectList(_context.Skills, "Id", "Name", credentialSkill.SkillId);
            return View(credentialSkill);
        }

        // GET: CredentialSkills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credentialSkill = await _context.CredentialSkills
                .Include(c => c.Credential)
                .Include(c => c.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credentialSkill == null)
            {
                return NotFound();
            }

            return View(credentialSkill);
        }

        // POST: CredentialSkills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var credentialSkill = await _context.CredentialSkills.FindAsync(id);
            if (credentialSkill != null)
            {
                _context.CredentialSkills.Remove(credentialSkill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CredentialSkillExists(int id)
        {
            return _context.CredentialSkills.Any(e => e.Id == id);
        }
    }
}
