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
    public class CredentialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CredentialsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User)!);
        }

        // GET: Admin/Credentials
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var credentials = await _context.Credentials
                .AsNoTracking()
                .GetSortedCredentialsForUser(userId)
                .ToListAsync();
            return View(credentials);
        }

        // GET: Admin/Credentials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
                .Include(c => c.CredentialSkills)
                    .ThenInclude(cs => cs.Skill)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (credential == null) return NotFound();

            return View(credential);
        }

        // GET: Admin/Credentials/Create
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

        // POST: Admin/Credentials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IssuingAuthority,IssueDate,ExpiryDate,VerificationUrl,BadgeUrl")] Credential credential, int[] selectedSkillIds)
        {
            var userId = GetCurrentUserId();
            credential.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            if (ModelState.IsValid)
            {
                _context.Add(credential);
                await _context.SaveChangesAsync();

                if (selectedSkillIds != null)
                {
                    foreach (var skillId in selectedSkillIds)
                    {
                        _context.CredentialSkills.Add(new CredentialSkill
                        {
                            CredentialId = credential.Id,
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
            return View(credential);
        }

        // GET: Admin/Credentials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.ApplicationUserId == userId);

            if (credential == null) return NotFound();

            ViewBag.Skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            ViewBag.SelectedSkillIds = await _context.CredentialSkills
                .AsNoTracking()
                .Where(cs => cs.CredentialId == credential.Id)
                .Select(cs => cs.SkillId)
                .ToListAsync();

            return View(credential);
        }

        // POST: Admin/Credentials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IssuingAuthority,IssueDate,ExpiryDate,VerificationUrl,BadgeUrl")] Credential credential, int[] selectedSkillIds)
        {
            if (id != credential.Id) return NotFound();

            var userId = GetCurrentUserId();
            credential.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credential);
                    await _context.SaveChangesAsync();

                    // Update CredentialSkills
                    var existingCredentialSkills = await _context.CredentialSkills
                        .Where(cs => cs.CredentialId == credential.Id)
                        .ToListAsync();
                    _context.CredentialSkills.RemoveRange(existingCredentialSkills);

                    if (selectedSkillIds != null)
                    {
                        foreach (var skillId in selectedSkillIds)
                        {
                            _context.CredentialSkills.Add(new CredentialSkill
                            {
                                CredentialId = credential.Id,
                                SkillId = skillId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialExists(credential.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Skills = await _context.Skills
                .AsNoTracking()
                .GetSortedSkillsForUser(userId)
                .ToListAsync();
            ViewBag.SelectedSkillIds = selectedSkillIds?.ToList() ?? new List<int>();
            return View(credential);
        }

        // GET: Admin/Credentials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (credential == null) return NotFound();

            return View(credential);
        }

        // POST: Admin/Credentials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
                .FirstOrDefaultAsync(c => c.Id == id && c.ApplicationUserId == userId);

            if (credential != null)
            {
                _context.Credentials.Remove(credential);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CredentialExists(int id, int userId)
        {
            return _context.Credentials.Any(e => e.Id == id && e.ApplicationUserId == userId);
        }
    }
}
