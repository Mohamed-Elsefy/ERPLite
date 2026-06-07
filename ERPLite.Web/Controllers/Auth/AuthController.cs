using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Shared.Helpers;
using ERPLite.Web.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Auth
{
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (await _authService.IsSignedInAsync(User))
            {
                return RedirectUserByRole();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var dto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Invalid email or password");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectUserByRole();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userId = User.GetUserId();
            await _authService.LogoutAsync(userId ?? string.Empty);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public IActionResult RedirectUserByRole()
        {
            if (User.IsAdmin())
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            if (User.IsManager())
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
        }
    }
}