using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Shared.Constants;
using ERPLite.Web.Models.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.HR
{
    [Authorize(Policy = "ManagerOnly")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;

        public DepartmentsController(IDepartmentService departmentService, IEmployeeService employeeService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
        }

        private int? GetManagerDepartmentId()
        {
            var claimValue = User.FindFirst("DepartmentId")?.Value;
            return int.TryParse(claimValue, out int id) ? id : null;
        }

        // GET: /Departments
        public async Task<IActionResult> Index(string search)
        {
            var result = await _departmentService.GetAllAsync();
            var departments = result.Data ?? new List<DepartmentDto>();

            if (User.IsInRole(Roles.Manager))
            {
                var managerDeptId = GetManagerDepartmentId();
                if (!managerDeptId.HasValue) return Forbid();

                departments = departments.Where(d => d.Id == managerDeptId.Value).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                departments = departments.Where(d =>
                    d.Name.ToLower().Contains(cleanSearch) ||
                    (d.Description != null && d.Description.ToLower().Contains(cleanSearch))
                ).ToList();
            }

            var viewModel = new DepartmentIndexViewModel
            {
                Departments = departments,
                SearchTerm = search
            };

            return View(viewModel);
        }

        // GET: /Departments/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            if (User.IsInRole(Roles.Manager))
            {
                var managerDeptId = GetManagerDepartmentId();
                if (!managerDeptId.HasValue || id != managerDeptId.Value)
                {
                    return Forbid();
                }
            }

            var result = await _departmentService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
                return NotFound();

            var empResult = await _employeeService.GetAllAsync();
            var departmentEmployees = (empResult.Data ?? new List<EmployeeDto>())
                .Where(e => e.DepartmentId == id).ToList();

            var viewModel = new DepartmentDetailsViewModel
            {
                DepartmentId = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                EmployeesCount = departmentEmployees.Count, 
                Employees = departmentEmployees
            };

            return View(viewModel);
        }

        #region Admin Only Actions 

        // GET: /Departments/Create
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View(new CreateDepartmentViewModel());
        }

        // POST: /Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateDepartmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = new CreateDepartmentDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _departmentService.CreateAsync(dto, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Departments/Edit/{id}
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _departmentService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
                return NotFound();

            var viewModel = new EditDepartmentViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description
            };

            return View(viewModel);
        }

        // POST: /Departments/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id, EditDepartmentViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = new UpdateDepartmentDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _departmentService.UpdateAsync(dto, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Departments/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _departmentService.DeleteAsync(id, currentUserId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
