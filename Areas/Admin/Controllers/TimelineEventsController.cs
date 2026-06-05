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
    public class TimelineEventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TimelineEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User)!);
        }

        // GET: Admin/TimelineEvents
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var events = await _context.TimelineEvents
                .AsNoTracking()
                .GetSortedTimelineEventsForUser(userId)
                .ToListAsync();
            return View(events);
        }

        // GET: Admin/TimelineEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var timelineEvent = await _context.TimelineEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (timelineEvent == null) return NotFound();

            return View(timelineEvent);
        }

        // GET: Admin/TimelineEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TimelineEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Organization,Location,Description,EventType,StartDate,EndDate")] TimelineEvent timelineEvent)
        {
            var userId = GetCurrentUserId();
            timelineEvent.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");


            if (ModelState.IsValid)
            {
                _context.Add(timelineEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timelineEvent);
        }

        // GET: Admin/TimelineEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var timelineEvent = await _context.TimelineEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == userId);

            if (timelineEvent == null) return NotFound();

            return View(timelineEvent);
        }

        // POST: Admin/TimelineEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Organization,Location,Description,EventType,StartDate,EndDate")] TimelineEvent timelineEvent)
        {
            if (id != timelineEvent.Id) return NotFound();

            var userId = GetCurrentUserId();
            timelineEvent.ApplicationUserId = userId;

            ModelState.Remove("ApplicationUser");


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timelineEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimelineEventExists(timelineEvent.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(timelineEvent);
        }

        // GET: Admin/TimelineEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            var timelineEvent = await _context.TimelineEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (timelineEvent == null) return NotFound();

            return View(timelineEvent);
        }

        // POST: Admin/TimelineEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var timelineEvent = await _context.TimelineEvents
                .FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == userId);

            if (timelineEvent != null)
            {
                _context.TimelineEvents.Remove(timelineEvent);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TimelineEventExists(int id, int userId)
        {
            return _context.TimelineEvents.Any(e => e.Id == id && e.ApplicationUserId == userId);
        }
    }
}
