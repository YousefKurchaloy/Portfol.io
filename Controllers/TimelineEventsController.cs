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
    public class TimelineEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimelineEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TimelineEvents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TimelineEvents.Include(t => t.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TimelineEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timelineEvent = await _context.TimelineEvents
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timelineEvent == null)
            {
                return NotFound();
            }

            return View(timelineEvent);
        }

        // GET: TimelineEvents/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio");
            return View();
        }

        // POST: TimelineEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Organization,Location,StartDate,EndDate,ApplicationUserId")] TimelineEvent timelineEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timelineEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio", timelineEvent.ApplicationUserId);
            return View(timelineEvent);
        }

        // GET: TimelineEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timelineEvent = await _context.TimelineEvents.FindAsync(id);
            if (timelineEvent == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio", timelineEvent.ApplicationUserId);
            return View(timelineEvent);
        }

        // POST: TimelineEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Organization,Location,StartDate,EndDate,ApplicationUserId")] TimelineEvent timelineEvent)
        {
            if (id != timelineEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timelineEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimelineEventExists(timelineEvent.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio", timelineEvent.ApplicationUserId);
            return View(timelineEvent);
        }

        // GET: TimelineEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timelineEvent = await _context.TimelineEvents
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timelineEvent == null)
            {
                return NotFound();
            }

            return View(timelineEvent);
        }

        // POST: TimelineEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timelineEvent = await _context.TimelineEvents.FindAsync(id);
            if (timelineEvent != null)
            {
                _context.TimelineEvents.Remove(timelineEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimelineEventExists(int id)
        {
            return _context.TimelineEvents.Any(e => e.Id == id);
        }
    }
}
