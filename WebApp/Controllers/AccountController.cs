using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SsgtContext _context;
        private readonly IMenuService _menuService;
        private readonly IErrorLogService _errorLogService;

        public AccountController(SsgtContext context, IMenuService menuService, IErrorLogService errorLogService)
        {
            _context = context;
            _menuService = menuService;
            _errorLogService = errorLogService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.FwUserDetails
                    .FirstOrDefault(u => u.UserName == model.UserName && u.IsActive == true);

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<FwUserDetail>();
                    var passwordValid = false;

                    if (string.IsNullOrWhiteSpace(user.PasswordHash))
                    {
                        // Existing WinForms accounts store the client-side SHA-256
                        // value in UserPassword. Migrate that value on first web login.
                        if (user.UserPassword == model.UserPassword)
                        {
                            user.PasswordHash = passwordHasher.HashPassword(user, model.UserPassword);
                            await _context.SaveChangesAsync();
                            passwordValid = true;
                        }
                    }
                    else
                    {
                        // Once migrated, UserPassword is ignored for web authentication.
                        var result = passwordHasher.VerifyHashedPassword(
                            user,
                            user.PasswordHash,
                            model.UserPassword);
                        passwordValid = result == PasswordVerificationResult.Success ||
                                        result == PasswordVerificationResult.SuccessRehashNeeded;
                    }

                    if (passwordValid)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim("FullName", user.UserName),
                            new Claim("UserId", user.UserId.ToString()),
                            new Claim(ClaimTypes.Role, user.RoleId?.ToString() ?? "User"),
                            new Claim("ContactNo", user.ContactNo?.ToString() ?? string.Empty)
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // Load menus once at login and store in session
                        await LoadMenusIntoSessionAsync(user.RoleId ?? 1);

                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.Error = "Invalid login attempt.";
            }
            else
                ViewBag.Error = "Invalid user name or password";
            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear menus from session
            HttpContext.Session.Remove("UserMenus");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Load menus once at login and store in session for subsequent page loads
        /// This prevents repeated database queries on every page navigation
        /// </summary>
        private async Task LoadMenusIntoSessionAsync(int userLevel)
        {
            try
            {
                var menus = await _menuService.GetMenusByUserLevelAsync(userLevel);

                // Serialize menus to JSON and store in session
                var menuJson = JsonSerializer.Serialize(menus);
                HttpContext.Session.SetString("UserMenus", menuJson);
            }
            catch (Exception ex)
            {
                // Log error but don't fail login
                _errorLogService.LogErrorAsync(
                    "Unknown",
                    "AccountController.LoadMenusIntoSessionAsync",
                    $"UserLevel={userLevel}",
                    ex).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"Error loading menus at login: {ex.Message}");
            }
        }
    }
}
