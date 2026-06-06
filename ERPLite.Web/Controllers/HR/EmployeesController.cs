using ERPLite.Data.Entities.Identity;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Shared.Constants;
using ERPLite.Web.Models.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.HR
{
    [Authorize(Policy = "ManagerOnly")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDashboardService _dashboardService;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            UserManager<ApplicationUser> userManager,
            IDashboardService dashboardService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _userManager = userManager;
            _dashboardService = dashboardService;
        }

        private int? GetManagerDepartmentId()
        {
            var claimValue = User.FindFirst("DepartmentId")?.Value;
            return int.TryParse(claimValue, out int id) ? id : null;
        }

        // GET: /Employees
        public async Task<IActionResult> Index()
        {
            var result = await _employeeService.GetAllAsync();
            var employees = result.Data ?? new List<EmployeeDto>();

            if (User.IsInRole(Roles.Manager))
            {
                var deptId = GetManagerDepartmentId();
                if (!deptId.HasValue) return Forbid();

                employees = employees.Where(e => e.DepartmentId == deptId.Value).ToList();
            }

            return View(employees);
        }

        // GET: /Employees/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _employeeService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Target employee profile not found.";
                return RedirectToAction(nameof(Index));
            }

            var empData = result.Data;

            if (User.IsInRole(Roles.Manager))
            {
                var deptId = GetManagerDepartmentId();
                if (!deptId.HasValue || empData.DepartmentId != deptId.Value)
                {
                    return Forbid();
                }
            }

            var attendanceResult = await _dashboardService.GetEmployeeHistoryAsync(id);

            var viewModel = new EmployeeDetailsViewModel
            {
                EmployeeId = empData.Id,
                FullName = empData.FullName,
                Email = empData.Email,
                Salary = empData.Salary,
                HireDate = empData.HireDate,
                DepartmentName = empData.DepartmentName,
                UserId = empData.UserId,
                AttendanceLogs = attendanceResult.Success ? attendanceResult.Data! : new List<ERPLite.Services.DTOs.Dashboard.AttendanceManagementDto>()
            };

            if (viewModel.HasSystemAccount)
            {
                var linkedUser = await _userManager.FindByIdAsync(viewModel.UserId!);
                if (linkedUser != null)
                {
                    var roles = await _userManager.GetRolesAsync(linkedUser);
                    viewModel.SystemRole = roles.FirstOrDefault() ?? "No Role Assigned";
                    viewModel.IsAccountLocked = await _userManager.IsLockedOutAsync(linkedUser);
                }
            }

            return View(viewModel);
        }

        #region Admin Only Actions

        // GET: /Employees/Create
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateEmployeeViewModel { DepartmentList = await GetDepartmentSelectListAsync() };
            return View(viewModel);
        }

        // POST: /Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateEmployeeViewModel viewModel)
        {
            if (viewModel.CreateUserAccount)
            {
                if (string.IsNullOrWhiteSpace(viewModel.Password))
                    ModelState.AddModelError(nameof(viewModel.Password), "Password is required when creating a system account.");
                if (string.IsNullOrWhiteSpace(viewModel.Role))
                    ModelState.AddModelError(nameof(viewModel.Role), "Please assign a security role.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                return View(viewModel);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (viewModel.CreateUserAccount)
            {
                var userExists = await _userManager.FindByEmailAsync(viewModel.Employee.Email);
                if (userExists != null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered to another user account.");
                    viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                    return View(viewModel);
                }
            }

            var result = await _employeeService.CreateAsync(viewModel.Employee, currentUserId);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                return View(viewModel);
            }

            var createdEmployeeResult = await _employeeService.GetAllAsync();
            var employeeData = createdEmployeeResult.Data?.FirstOrDefault(x => x.Email == viewModel.Employee.Email);

            if (viewModel.CreateUserAccount && employeeData != null)
            {
                var newUser = new ApplicationUser
                {
                    FullName = employeeData.FullName,
                    Email = employeeData.Email,
                    UserName = employeeData.Email,
                    EmployeeId = employeeData.Id,
                    CreatedAt = DateTime.UtcNow
                };

                var createAccountResult = await _userManager.CreateAsync(newUser, viewModel.Password!);
                if (createAccountResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, viewModel.Role!);

                    var updateDto = new UpdateEmployeeDto
                    {
                        Id = employeeData.Id,
                        FullName = employeeData.FullName,
                        Email = employeeData.Email,
                        Salary = employeeData.Salary,
                        DepartmentId = employeeData.DepartmentId,
                        UserId = newUser.Id
                    };
                    await _employeeService.UpdateAsync(updateDto, currentUserId);
                }
                else
                {
                    TempData["Warning"] = "Personnel profile created, but user account synchronization failed.";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Success"] = "Employee data updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Edit/{id}
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _employeeService.GetByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var viewModel = new EditEmployeeViewModel
            {
                Employee = new UpdateEmployeeDto
                {
                    Id = result.Data.Id,
                    FullName = result.Data.FullName,
                    Email = result.Data.Email,
                    Salary = result.Data.Salary,
                    DepartmentId = result.Data.DepartmentId,
                    UserId = result.Data.UserId
                },
                DepartmentList = await GetDepartmentSelectListAsync()
            };
            return View(viewModel);
        }

        // POST: /Employees/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(EditEmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                return View(viewModel);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _employeeService.UpdateAsync(viewModel.Employee, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Employees/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var employeeResult = await _employeeService.GetByIdAsync(id);
            if (employeeResult.Success && employeeResult.Data != null)
            {
                var employeeData = employeeResult.Data;

                if (!string.IsNullOrEmpty(employeeData.UserId))
                {
                    var linkedUser = await _userManager.FindByIdAsync(employeeData.UserId);
                    if (linkedUser != null)
                    {
                        if (linkedUser.Id == currentUserId)
                        {
                            TempData["Error"] = "Protection Shield: You cannot delete the profile of the currently logged-in administrator.";
                            return RedirectToAction(nameof(Index));
                        }

                        var updateDto = new UpdateEmployeeDto
                        {
                            Id = employeeData.Id,
                            FullName = employeeData.FullName,
                            Email = employeeData.Email,
                            Salary = employeeData.Salary,
                            DepartmentId = employeeData.DepartmentId,
                            UserId = null
                        };
                        await _employeeService.UpdateAsync(updateDto, currentUserId);
                        await _userManager.DeleteAsync(linkedUser);
                    }
                }
            }

            var result = await _employeeService.DeleteAsync(id, currentUserId);
            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        #endregion

        private async Task<SelectList> GetDepartmentSelectListAsync()
        {
            var result = await _departmentService.GetAllAsync();
            return new SelectList(result.Data ?? new List<DepartmentDto>(), "Id", "Name");
        }
    }
}
