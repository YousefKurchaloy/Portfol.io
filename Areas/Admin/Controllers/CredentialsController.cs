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
                .Where(c => c.ApplicationUserId == userId)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();
            return View(credentials);
        }

        // GET: Admin/Credentials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (credential == null) return NotFound();

            return View(credential);
        }

        // GET: Admin/Credentials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Credentials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IssuingAuthority,IssueDate,ExpiryDate,VerificationUrl,BadgeUrl")] Credential credential)
        {
            var userId = GetCurrentUserId();
            credential.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");

            if (ModelState.IsValid)
            {
                _context.Add(credential);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(credential);
        }

        // GET: Admin/Credentials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
                .FirstOrDefaultAsync(c => c.Id == id && c.ApplicationUserId == userId);

            if (credential == null) return NotFound();

            return View(credential);
        }

        // POST: Admin/Credentials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IssuingAuthority,IssueDate,ExpiryDate,VerificationUrl,BadgeUrl")] Credential credential)
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialExists(credential.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(credential);
        }

        // GET: Admin/Credentials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var credential = await _context.Credentials
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
