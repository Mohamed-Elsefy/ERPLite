using AutoMapper;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Models.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index(string keyword)
        {
            var result = string.IsNullOrWhiteSpace(keyword)
                ? await _customerService.GetAllAsync()
                : await _customerService.SearchAsync(keyword);

            var customersData = result.Data ?? new List<CustomerDto>();

            if (_currentUser.IsManager)
            {
                var deptId = _currentUser.DepartmentId;
                if (!deptId.HasValue) return Forbid();

                customersData = customersData.Where(c => c.Orders.Any(o => o.DepartmentId == deptId.Value)).ToList();
            }

            var viewModel = new CustomerIndexViewModel
            {
                Customers = customersData,
                SearchTerm = keyword
            };

            return View(viewModel);
        }

        // GET: /Customers/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Target customer profile not found.";
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

            TempData["Success"] = result.Message;
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

            TempData["Success"] = result.Message;
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
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}