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
    public class PlatformProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlatformProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User)!);
        }

        // GET: Admin/PlatformProfiles
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var profiles = await _context.PlatformProfiles
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == userId)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return View(profiles);
        }

        // GET: Admin/PlatformProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var profile = await _context.PlatformProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (profile == null) return NotFound();

            return View(profile);
        }

        // GET: Admin/PlatformProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PlatformProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlatformName,UserHandle,Rank,ProfileUrl,DisplayOrder")] PlatformProfile profile)
        {
            var userId = GetCurrentUserId();
            profile.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            // Uniqueness check for DisplayOrder
            if (await _context.PlatformProfiles.AnyAsync(p => p.ApplicationUserId == userId && p.DisplayOrder == profile.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "This Display Order is already in use by another platform profile.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Admin/PlatformProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var profile = await _context.PlatformProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == userId);

            if (profile == null) return NotFound();

            return View(profile);
        }

        // POST: Admin/PlatformProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlatformName,UserHandle,Rank,ProfileUrl,DisplayOrder")] PlatformProfile profile)
        {
            if (id != profile.Id) return NotFound();

            var userId = GetCurrentUserId();
            profile.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            // Uniqueness check for DisplayOrder
            if (await _context.PlatformProfiles.AnyAsync(p => p.ApplicationUserId == userId && p.DisplayOrder == profile.DisplayOrder && p.Id != profile.Id))
            {
                ModelState.AddModelError("DisplayOrder", "This Display Order is already in use by another platform profile.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatformProfileExists(profile.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Admin/PlatformProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var profile = await _context.PlatformProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (profile == null) return NotFound();

            return View(profile);
        }

        // POST: Admin/PlatformProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var profile = await _context.PlatformProfiles
                .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == userId);

            if (profile != null)
            {
                _context.PlatformProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlatformProfileExists(int id, int userId)
        {
            return _context.PlatformProfiles.Any(e => e.Id == id && e.ApplicationUserId == userId);
        }
    }
}
