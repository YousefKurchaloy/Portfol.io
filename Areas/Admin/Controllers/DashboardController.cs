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
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(_userManager.GetUserId(User)!);

            ViewBag.ProjectCount = await _context.Projects.CountAsync(p => p.ApplicationUserId == userId);
            ViewBag.SkillCount = await _context.Skills.CountAsync(s => s.ApplicationUserId == userId);
            ViewBag.CredentialCount = await _context.Credentials.CountAsync(c => c.ApplicationUserId == userId);
            ViewBag.TimelineCount = await _context.TimelineEvents.CountAsync(e => e.ApplicationUserId == userId);
            ViewBag.ProfileCount = await _context.PlatformProfiles.CountAsync(p => p.ApplicationUserId == userId);
            
            ViewBag.TotalMessages = await _context.ContactMessages.CountAsync(m => m.ApplicationUserId == userId);
            ViewBag.UnreadMessages = await _context.ContactMessages.CountAsync(m => m.ApplicationUserId == userId && !m.IsRead);

            // Get recent projects
            var recentProjects = await _context.Projects
                .Where(p => p.ApplicationUserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Get recent messages
            var recentMessages = await _context.ContactMessages
                .Where(m => m.ApplicationUserId == userId && !m.IsArchived)
                .OrderByDescending(m => m.SentDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentProjects = recentProjects;
            ViewBag.RecentMessages = recentMessages;

            return View();
        }
    }
}
