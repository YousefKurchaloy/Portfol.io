using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using Portfolio.Areas.Admin.ViewModels;

namespace Portfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Admin/Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Logged-in user details not found.");
            }

            var model = new EditProfileViewModel
            {
                FullName = user.FullName ?? string.Empty,
                JobTitle = user.JobTitle,
                Bio = user.Bio,
                ProfileImageUrl = user.ProfileImageUrl,
                GitHubUrl = user.GitHubUrl,
                LinkedInUrl = user.LinkedInUrl,
                AvailabilityStatus = user.AvailabilityStatus
            };

            return View(model);
        }

        // POST: Admin/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Logged-in user details not found.");
            }

            user.FullName = model.FullName;
            user.JobTitle = model.JobTitle;
            user.Bio = model.Bio;
            user.ProfileImageUrl = model.ProfileImageUrl;
            user.GitHubUrl = model.GitHubUrl;
            user.LinkedInUrl = model.LinkedInUrl;
            user.AvailabilityStatus = model.AvailabilityStatus;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Refresh sign-in cookies since the user claims (like FullName/UserName if changed) might have updated
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Profile info successfully updated.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
