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
    public async Task<IActionResult> Index(string? username)
    {
        if (string.IsNullOrEmpty(username) || username.Equals("Home", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound("Portfolio username is required.");
        }

        ApplicationUser? targetUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (targetUser == null)
        {
            return NotFound($"Portfolio profile for user '{username}' was not found.");
        }

        var model = new HomeViewModel();

        if (targetUser != null)
        {
            model.AdminUser = targetUser;
            model.TargetUserId = targetUser.Id;
            model.TargetUsername = targetUser.UserName ?? string.Empty;

            model.Projects = await _context.Projects
                .Include(p => p.ProjectSkills)
                    .ThenInclude(ps => ps.Skill)
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == targetUser.Id)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CompletionDate)
                .ToListAsync();

            model.Skills = await _context.Skills
                .AsNoTracking()
                .Where(s => s.ApplicationUserId == targetUser.Id)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.DisplayOrder)
                .ToListAsync();

            model.Credentials = await _context.Credentials
                .Include(c => c.CredentialSkills)
                    .ThenInclude(cs => cs.Skill)
                .AsNoTracking()
                .Where(c => c.ApplicationUserId == targetUser.Id)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            model.TimelineEvents = await _context.TimelineEvents
                .AsNoTracking()
                .Where(e => e.ApplicationUserId == targetUser.Id)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();

            model.PlatformProfiles = await _context.PlatformProfiles
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == targetUser.Id)
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
        var targetUser = await _userManager.FindByIdAsync(model.TargetUserId.ToString());
        if (targetUser == null)
        {
            ModelState.AddModelError(string.Empty, "Target portfolio owner profile not found. Message routing failed.");
        }

        if (ModelState.IsValid && targetUser != null)
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
                ApplicationUserId = targetUser.Id
            };

            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Message successfully submitted to {targetUser.FullName}'s Inbox.";
            return RedirectToRoute("user_portfolio", new { username = targetUser.UserName });
        }

        // If validation fails, re-hydrate view model
        if (targetUser != null)
        {
            model.AdminUser = targetUser;
            model.TargetUserId = targetUser.Id;
            model.TargetUsername = targetUser.UserName ?? string.Empty;

            model.Projects = await _context.Projects
                .Include(p => p.ProjectSkills)
                    .ThenInclude(ps => ps.Skill)
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == targetUser.Id)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CompletionDate)
                .ToListAsync();

            model.Skills = await _context.Skills
                .AsNoTracking()
                .Where(s => s.ApplicationUserId == targetUser.Id)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.DisplayOrder)
                .ToListAsync();

            model.Credentials = await _context.Credentials
                .Include(c => c.CredentialSkills)
                    .ThenInclude(cs => cs.Skill)
                .AsNoTracking()
                .Where(c => c.ApplicationUserId == targetUser.Id)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            model.TimelineEvents = await _context.TimelineEvents
                .AsNoTracking()
                .Where(e => e.ApplicationUserId == targetUser.Id)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();

            model.PlatformProfiles = await _context.PlatformProfiles
                .AsNoTracking()
                .Where(p => p.ApplicationUserId == targetUser.Id)
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
