using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.Models;
using Portfolio.Areas.Admin.ViewModels;

namespace Portfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // ─────────────────────────────────────────────
        // LOGIN
        // ─────────────────────────────────────────────

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // Prevent users who are already logged in from seeing the login page
            if (User.Identity is { IsAuthenticated: true })
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt to sign in via Identity cryptography
            // lockoutOnFailure ensures bruteforce attacks are stopped after max attempts
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Admin session initiated.");
                return RedirectToSafeUrl(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Admin account locked out due to multiple failed attempts.");
                ModelState.AddModelError(string.Empty, "Account locked. Please wait before trying again.");
                return View(model);
            }
            else
            {
                // We deliberately use a generic error message so attackers don't know 
                // if they guessed the email right but the password wrong.
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        // ─────────────────────────────────────────────
        // REGISTER
        // ─────────────────────────────────────────────

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // Prevent users who are already logged in from seeing the register page
            if (User.Identity is { IsAuthenticated: true })
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Bio = "New admin user. Update this bio from the dashboard.",
                AvailabilityStatus = EAvailabilityStatus.Building,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("New admin account created for {Email}.", model.Email);

                // Auto-assign Admin role
                await _userManager.AddToRoleAsync(user, "Admin");

                // Auto-sign-in after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            // Surface Identity errors (e.g., password too weak, email taken)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ─────────────────────────────────────────────
        // LOGOUT
        // ─────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Admin session terminated.");

            // Redirect back to the public-facing portfolio homepage
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // ─────────────────────────────────────────────
        // ACCESS DENIED
        // ─────────────────────────────────────────────

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            // Renders when an unauthenticated user tries to hit a protected route
            return View();
        }

        // ─────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Prevents "Open Redirect" attacks where a malicious link 
        /// tricks the server into redirecting the user to an external phishing site.
        /// </summary>
        private IActionResult RedirectToSafeUrl(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
    }
}