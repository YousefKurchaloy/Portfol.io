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
    public class ContactMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactMessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User)!);
        }

        // GET: Admin/ContactMessages
        public async Task<IActionResult> Index(bool showArchived = false)
        {
            var userId = GetCurrentUserId();
            var messages = await _context.ContactMessages
                .AsNoTracking()
                .Where(m => m.ApplicationUserId == userId && m.IsArchived == showArchived)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            ViewData["ShowArchived"] = showArchived;
            return View(messages);
        }

        // GET: Admin/ContactMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (message == null) return NotFound();

            // Auto-mark as read when opened
            if (!message.IsRead)
            {
                message.IsRead = true;
                _context.Update(message);
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        // POST: Admin/ContactMessages/ToggleArchive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleArchive(int id)
        {
            var userId = GetCurrentUserId();
            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (message == null) return NotFound();

            message.IsArchived = !message.IsArchived;
            _context.Update(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { showArchived = !message.IsArchived });
        }

        // GET: Admin/ContactMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var message = await _context.ContactMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (message == null) return NotFound();

            return View(message);
        }

        // POST: Admin/ContactMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
