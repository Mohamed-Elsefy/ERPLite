using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.DTOs.Auth;
using ERPLite.Web.Areas.Admin.Models.Users;
using ERPLite.Shared.Helpers;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var result = await _userService.GetFilteredUsersAsync(search);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(new UserIndexViewModel { SearchTerm = search });
            }

            var viewModel = new UserIndexViewModel
            {
                SearchTerm = search,
                Users = (result.Data ?? Enumerable.Empty<UserDto>()).Select(user => new UserListViewModel
                {
                    Id = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    IsLocked = user.IsLocked
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CreateUserDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            var currentUserId = User.GetUserId()?.ToString() ?? string.Empty;
            var result = await _userService.CreateUserAsync(dto, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var currentUserId = User.GetUserId()?.ToString() ?? string.Empty;

            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Self-lockout protective block triggered. You cannot lock yourself.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userService.LockUserAsync(id, currentUserId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var currentUserId = User.GetUserId()?.ToString() ?? string.Empty;
            var result = await _userService.UnlockUserAsync(id, currentUserId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            var model = new EditUserRoleViewModel
            {
                UserId = result.Data.UserId,
                FullName = result.Data.FullName,
                Email = result.Data.Email,
                Role = result.Data.Role
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditUserRoleViewModel model)
        {
            ModelState.Remove(nameof(model.FullName));
            ModelState.Remove(nameof(model.Email));

            if (!ModelState.IsValid)
                return View(model);

            var dto = new UpdateUserRoleDto
            {
                UserId = model.UserId,
                Role = model.Role
            };

            var currentUserId = User.GetUserId()?.ToString() ?? string.Empty;
            var result = await _userService.UpdateUserRoleAsync(dto, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}