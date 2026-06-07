using AutoMapper;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Models.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPLite.Web.Controllers.HR
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeAccountService _employeeAccountService;
        private readonly IDepartmentService _departmentService;
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public EmployeesController(
            IEmployeeService employeeService,
            IEmployeeAccountService employeeAccountService,
            IDepartmentService departmentService,
            IDashboardService dashboardService,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _employeeService = employeeService;
            _employeeAccountService = employeeAccountService;
            _departmentService = departmentService;
            _dashboardService = dashboardService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // GET: /Employees
        public async Task<IActionResult> Index()
        {
            var result = await _employeeService.GetAllAsync();
            var employees = result.Data ?? new List<EmployeeDto>();

            if (_currentUser.IsManager)
            {
                var deptId = _currentUser.DepartmentId;
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

            if (_currentUser.IsManager)
            {
                var deptId = _currentUser.DepartmentId;
                if (!deptId.HasValue || empData.DepartmentId != deptId.Value)
                {
                    return Forbid();
                }
            }

            var attendanceResult = await _dashboardService.GetEmployeeHistoryAsync(id);
            var viewModel = _mapper.Map<EmployeeDetailsViewModel>(result.Data);

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
            if (!ModelState.IsValid)
            {
                viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                return View(viewModel);
            }

            var result = await _employeeAccountService.CreateEmployeeWithAccountAsync(
                viewModel.Employee,
                viewModel.Password,
                viewModel.Role,
                viewModel.CreateUserAccount,
                _currentUser.UserId!
            );

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                viewModel.DepartmentList = await GetDepartmentSelectListAsync();
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Edit/{id}
        [Authorize(Policy = "AdminOnly")] 
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _employeeService.GetByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var viewModel = _mapper.Map<EditEmployeeViewModel>(result.Data);
            viewModel.DepartmentList = await GetDepartmentSelectListAsync();

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

            var result = await _employeeService.UpdateAsync(viewModel.Employee, _currentUser.UserId!);

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
            var employeeResult = await _employeeService.GetByIdAsync(id);
            if (employeeResult.Success && employeeResult.Data != null)
            {
                // حماية أمنية: منع المسؤول الفعال من تدمير حسابه الخاص
                if (employeeResult.Data.UserId == _currentUser.UserId)
                {
                    TempData["Error"] = "Protection Shield: You cannot delete the profile of the currently logged-in administrator.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var result = await _employeeService.DeleteAsync(id, _currentUser.UserId!);

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