using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Home/Index
    public async Task<IActionResult> Index()
    {
        var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();
        var model = new HomeViewModel();

        if (adminUser != null)
        {
            model.AdminUser = adminUser;
            model.Projects = await _context.Projects
                .Where(p => p.ApplicationUserId == adminUser.Id)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CompletionDate)
                .ToListAsync();

            model.Skills = await _context.Skills
                .Where(s => s.ApplicationUserId == adminUser.Id)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.DisplayOrder)
                .ToListAsync();

            model.Credentials = await _context.Credentials
                .Where(c => c.ApplicationUserId == adminUser.Id)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            model.TimelineEvents = await _context.TimelineEvents
                .Where(e => e.ApplicationUserId == adminUser.Id)
                .OrderBy(e => e.DisplayOrder)
                .ThenByDescending(e => e.StartDate)
                .ToListAsync();

            model.PlatformProfiles = await _context.PlatformProfiles
                .Where(p => p.ApplicationUserId == adminUser.Id)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
        }

        return View(model);
    }

    // POST: Home/SubmitMessage
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitMessage(HomeViewModel model)
    {
        var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();
        if (adminUser == null)
        {
            ModelState.AddModelError(string.Empty, "System administrator profile not found. Message routing failed.");
        }

        if (ModelState.IsValid && adminUser != null)
        {
            var contactMessage = new ContactMessage
            {
                SenderName = model.SenderName,
                SenderEmail = model.SenderEmail,
                Subject = model.Subject,
                Body = model.Body,
                SentDate = DateTime.UtcNow,
                IsRead = false,
                IsArchived = false,
                ApplicationUserId = adminUser.Id
            };

            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message successfully submitted to Admin Inbox.";
            return RedirectToAction(nameof(Index));
        }

        // If validation fails, re-hydrate view model
        if (adminUser != null)
        {
            model.AdminUser = adminUser;
            model.Projects = await _context.Projects
                .Where(p => p.ApplicationUserId == adminUser.Id)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CompletionDate)
                .ToListAsync();

            model.Skills = await _context.Skills
                .Where(s => s.ApplicationUserId == adminUser.Id)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.DisplayOrder)
                .ToListAsync();

            model.Credentials = await _context.Credentials
                .Where(c => c.ApplicationUserId == adminUser.Id)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            model.TimelineEvents = await _context.TimelineEvents
                .Where(e => e.ApplicationUserId == adminUser.Id)
                .OrderBy(e => e.DisplayOrder)
                .ThenByDescending(e => e.StartDate)
                .ToListAsync();

            model.PlatformProfiles = await _context.PlatformProfiles
                .Where(p => p.ApplicationUserId == adminUser.Id)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
        }

        return View("Index", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
