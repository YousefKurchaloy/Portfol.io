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
        await PopulatePortfolioDataAsync(model, targetUser);
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
            await PopulatePortfolioDataAsync(model, targetUser);
        }

        return View("Index", model);
    }

    private async Task PopulatePortfolioDataAsync(HomeViewModel model, ApplicationUser targetUser)
    {
        model.AdminUser = targetUser;
        model.TargetUserId = targetUser.Id;
        model.TargetUsername = targetUser.UserName ?? string.Empty;

        model.Projects = await _context.Projects
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .AsNoTracking()
            .GetSortedProjectsForUser(targetUser.Id)
            .ToListAsync();

        model.Skills = await _context.Skills
            .AsNoTracking()
            .GetSortedSkillsForUser(targetUser.Id)
            .ToListAsync();

        model.Credentials = await _context.Credentials
            .Include(c => c.CredentialSkills)
                .ThenInclude(cs => cs.Skill)
            .AsNoTracking()
            .GetSortedCredentialsForUser(targetUser.Id)
            .ToListAsync();

        model.TimelineEvents = await _context.TimelineEvents
            .AsNoTracking()
            .GetSortedTimelineEventsForUser(targetUser.Id)
            .ToListAsync();

        model.PlatformProfiles = await _context.PlatformProfiles
            .AsNoTracking()
            .GetSortedPlatformProfilesForUser(targetUser.Id)
            .ToListAsync();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
