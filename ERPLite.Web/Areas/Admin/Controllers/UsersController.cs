using AutoMapper;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.DTOs.Users;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.Users;
using ERPLite.Shared.Helpers;
using ERPLite.Web.Areas.Admin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")] 
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserService userService,
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            IMapper mapper)
        {
            _userService = userService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(string? search)
        {
            var result = await _userService.GetFilteredUsersAsync(search);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(new UserIndexViewModel { SearchTerm = search });
            }

            var viewModel = new UserIndexViewModel
            {
                SearchTerm = search,
                Users = _mapper.Map<List<UserListViewModel>>(result.Data ?? Enumerable.Empty<UserDto>())
            };

            return View(viewModel);
        }

        // GET: /Admin/Users/Create
        public async Task<IActionResult> Create()
        {
            var model = new CreateUserViewModel
            {
                DepartmentList = await GetDepartmentSelectListAsync()
            };
            return View(model);
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DepartmentList = await GetDepartmentSelectListAsync();
                return View(model);
            }

            if (!model.IsEmployee)
            {
                ModelState.AddModelError(string.Empty, "External standard portals are under maintenance. Standalone users must be flagged as internal employees.");
                model.DepartmentList = await GetDepartmentSelectListAsync();
                return View(model);
            }

            var dto = _mapper.Map<CreateUserDto>(model);
            var currentUserId = User.GetUserId() ?? string.Empty;
            var result = await _userService.CreateUserAsync(dto, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                model.DepartmentList = await GetDepartmentSelectListAsync();
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/Promote
        public async Task<IActionResult> Promote()
        {
            var model = new PromoteEmployeeViewModel
            {
                UnlinkedEmployeesList = await GetUnlinkedEmployeesSelectListAsync()
            };
            return View(model);
        }

        // POST: /Admin/Users/Promote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(PromoteEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UnlinkedEmployeesList = await GetUnlinkedEmployeesSelectListAsync();
                return View(model);
            }

            var currentUserId = User.GetUserId() ?? string.Empty;
            var result = await _userService.GrantAccessToExistingEmployeeAsync(
                model.EmployeeId,
                model.Password,
                model.Role,
                currentUserId
            );

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                model.UnlinkedEmployeesList = await GetUnlinkedEmployeesSelectListAsync();
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Users/Lock/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var currentUserId = User.GetUserId() ?? string.Empty;

            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Self-lockout protective block triggered. You cannot suspend your own admin session.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userService.LockUserAsync(id, currentUserId);
            if (!result.Success) TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Users/Unlock/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var currentUserId = User.GetUserId() ?? string.Empty;
            var result = await _userService.UnlockUserAsync(id, currentUserId);

            if (!result.Success) TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/EditRole/{id}
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var model = new EditUserRoleViewModel
            {
                UserId = result.Data.UserId,
                FullName = result.Data.FullName,
                Email = result.Data.Email,
                Role = result.Data.Role
            };

            return View(model);
        }

        // POST: /Admin/Users/EditRole/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditUserRoleViewModel model)
        {
            ModelState.Remove(nameof(model.FullName));
            ModelState.Remove(nameof(model.Email));

            if (!ModelState.IsValid) return View(model);

            var currentUserId = User.GetUserId() ?? string.Empty;
            ServiceResult result;

            if (string.IsNullOrWhiteSpace(model.Role) || model.Role.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                result = await _userService.RemoveUserAccessAsync(model.UserId, currentUserId);
            }
            else
            {
                var dto = new UpdateUserRoleDto
                {
                    UserId = model.UserId,
                    Role = model.Role
                };
                result = await _userService.UpdateUserRoleAsync(dto, currentUserId);
            }

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var userData = result.Data;

            var viewModel = new UserDetailsViewModel
            {
                UserId = userData.UserId,
                Email = userData.Email,
                FullName = userData.FullName,
                Role = userData.Role,
                IsLocked = userData.IsLocked,
                AccountCreatedAt = userData.CreatedAt
            };

            if (userData.EmployeeId.HasValue)
            {
                var empResult = await _employeeService.GetByIdAsync(userData.EmployeeId.Value);
                if (empResult.Success && empResult.Data != null)
                {
                    viewModel.EmployeeId = empResult.Data.Id;
                    viewModel.DepartmentName = empResult.Data.DepartmentName;
                    viewModel.Salary = empResult.Data.Salary;
                    viewModel.HireDate = empResult.Data.HireDate;
                }
            }

            return View(viewModel);
        }

        private async Task<SelectList> GetDepartmentSelectListAsync()
        {
            var result = await _departmentService.GetAllAsync();
            return new SelectList(result.Data ?? new List<DepartmentDto>(), "Id", "Name");
        }

        private async Task<SelectList> GetUnlinkedEmployeesSelectListAsync()
        {
            var result = await _employeeService.GetAllAsync();
            var unlinkedList = (result.Data ?? new List<EmployeeDto>())
                .Where(e => string.IsNullOrEmpty(e.UserId))
                .Select(e => new { e.Id, Info = $"{e.FullName} ({e.Email})" })
                .ToList();

            return new SelectList(unlinkedList, "Id", "Info");
        }
    }
}