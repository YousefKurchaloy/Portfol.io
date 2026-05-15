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
    public class PlatformProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlatformProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PlatformProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PlatformProfiles.Include(p => p.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PlatformProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformProfile = await _context.PlatformProfiles
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platformProfile == null)
            {
                return NotFound();
            }

            return View(platformProfile);
        }

        // GET: PlatformProfiles/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio");
            return View();
        }

        // POST: PlatformProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlatformName,UserHandle,Rank,ApplicationUserId")] PlatformProfile platformProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(platformProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio", platformProfile.ApplicationUserId);
            return View(platformProfile);
        }

        // GET: PlatformProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformProfile = await _context.PlatformProfiles.FindAsync(id);
            if (platformProfile == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio", platformProfile.ApplicationUserId);
            return View(platformProfile);
        }

        // POST: PlatformProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlatformName,UserHandle,Rank,ApplicationUserId")] PlatformProfile platformProfile)
        {
            if (id != platformProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(platformProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatformProfileExists(platformProfile.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Bio", platformProfile.ApplicationUserId);
            return View(platformProfile);
        }

        // GET: PlatformProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platformProfile = await _context.PlatformProfiles
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (platformProfile == null)
            {
                return NotFound();
            }

            return View(platformProfile);
        }

        // POST: PlatformProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var platformProfile = await _context.PlatformProfiles.FindAsync(id);
            if (platformProfile != null)
            {
                _context.PlatformProfiles.Remove(platformProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatformProfileExists(int id)
        {
            return _context.PlatformProfiles.Any(e => e.Id == id);
        }
    }
}
