using AutoMapper;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Models.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.HR
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public DepartmentsController(IDepartmentService departmentService, ICurrentUserService currentUser, IMapper mapper)
        {
            _departmentService = departmentService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // GET: /Departments
        public async Task<IActionResult> Index(string search)
        {
            var result = await _departmentService.GetAllAsync();
            var departments = result.Data ?? new List<DepartmentDto>();

            if (_currentUser.IsManager)
            {
                var managerDeptId = _currentUser.DepartmentId;
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
            if (_currentUser.IsManager)
            {
                var managerDeptId = _currentUser.DepartmentId;
                if (!managerDeptId.HasValue || id != managerDeptId.Value)
                {
                    return Forbid(); 
                }
            }

            var result = await _departmentService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
                return NotFound();

            var viewModel = _mapper.Map<DepartmentDetailsViewModel>(result.Data);

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

            var result = await _departmentService.CreateAsync(dto, _currentUser.UserId!);

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

            var result = await _departmentService.UpdateAsync(dto, _currentUser.UserId!);

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
            var result = await _departmentService.DeleteAsync(id, _currentUser.UserId!);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}