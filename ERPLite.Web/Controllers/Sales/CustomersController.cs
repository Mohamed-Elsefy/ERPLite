using AutoMapper;
using ERPLite.Services.DTOs.Common;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Models.Common;
using ERPLite.Web.Models.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERPLite.Shared.Constants;

namespace ERPLite.Web.Controllers.Sales
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, ICurrentUserService currentUser, IMapper mapper)
        {
            _customerService = customerService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // GET: /Customers
        public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = 10)
        {
            int? departmentId = null;
            if (_currentUser.IsManager)
            {
                departmentId = _currentUser.DepartmentId;
                if (!departmentId.HasValue) return Forbid();
            }

            var request = new PagedRequestDto { PageNumber = page, PageSize = pageSize, SearchTerm = keyword };
            var result = await _customerService.GetPagedAsync(request, departmentId);
            var paged = result.Data ?? new PagedResponseDto<CustomerDto>();

            var filters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(keyword))
                filters["keyword"] = keyword;

            var viewModel = new CustomerIndexViewModel
            {
                Customers = paged.Items,
                SearchTerm = keyword,
                Pagination = PaginationViewModel.FromPagedResponse(paged, filters)
            };

            return View(viewModel);
        }

        // GET: /Customers/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData[UiKeys.Error] = "Target customer profile not found.";
                return RedirectToAction(nameof(Index));
            }

            if (_currentUser.IsManager)
            {
                var deptId = _currentUser.DepartmentId;
                if (!deptId.HasValue || !result.Data.Orders.Any(o => o.DepartmentId == deptId.Value))
                {
                    return Forbid();
                }
            }

            var viewModel = _mapper.Map<CustomerDetailsViewModel>(result.Data);
            return View(viewModel);
        }

        // GET: /Customers/Create
        public IActionResult Create()
        {
            return View(new CreateCustomerViewModel());
        }

        // POST: /Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var dto = _mapper.Map<CreateCustomerDto>(viewModel);
            var result = await _customerService.CreateAsync(dto, _currentUser.UserId!);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(viewModel);
            }

            TempData[UiKeys.Success] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        #region Admin & Manager Privileged Actions

        // GET: /Customers/Edit/{id}
        [Authorize(Policy = "RequireManagerOrAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            if (_currentUser.IsManager)
            {
                var deptId = _currentUser.DepartmentId;
                if (!deptId.HasValue || !result.Data.Orders.Any(o => o.DepartmentId == deptId.Value)) return Forbid();
            }

            var viewModel = _mapper.Map<EditCustomerViewModel>(result.Data);
            return View(viewModel);
        }

        // POST: /Customers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireManagerOrAdmin")]
        public async Task<IActionResult> Edit(EditCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var dto = _mapper.Map<UpdateCustomerDto>(viewModel);
            var result = await _customerService.UpdateAsync(dto, _currentUser.UserId!);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(viewModel);
            }

            TempData[UiKeys.Success] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Admin Only Sovereign Actions

        // POST: /Customers/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id, _currentUser.UserId!);
            TempData[result.Success ? UiKeys.Success : UiKeys.Error] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}