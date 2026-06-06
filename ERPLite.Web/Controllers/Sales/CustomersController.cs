using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Shared.Constants;
using ERPLite.Web.Models.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.Sales
{
    [Authorize(Policy = "AllUsers")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private int? GetManagerDepartmentId()
        {
            var claimValue = User.FindFirst("DepartmentId")?.Value;
            return int.TryParse(claimValue, out int id) ? id : null;
        }

        // GET: /Customers
        public async Task<IActionResult> Index(string keyword)
        {
            var result = string.IsNullOrWhiteSpace(keyword)
                ? await _customerService.GetAllAsync()
                : await _customerService.SearchAsync(keyword);

            var customersData = result.Data ?? new List<CustomerDto>();

            if (User.IsInRole(Roles.Manager) && !User.IsInRole(Roles.Admin))
            {
                var deptId = GetManagerDepartmentId();
                if (!deptId.HasValue) return Forbid();
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

            var customer = result.Data;

            var viewModel = new CustomerDetailsViewModel
            {
                CustomerId = customer.Id,
                FullName = customer.FullName,
                Phone = customer.Phone,
                OnboardedAt = customer.CreatedAt,
                OrdersPipeline = customer.Orders ?? new List<OrderDto>()
            };

            return View(viewModel);
        }

        // GET: /Customers/Create
        public IActionResult Create()
        {
            var viewModel = new CreateCustomerViewModel();
            return View(viewModel);
        }

        // POST: /Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var dto = new CreateCustomerDto
            {
                FullName = viewModel.FullName,
                Phone = viewModel.Phone
            };

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _customerService.CreateAsync(dto, currentUserId);

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
        [Authorize(Policy = "ManagerOnly")] 
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var viewModel = new EditCustomerViewModel
            {
                Id = result.Data.Id,
                FullName = result.Data.FullName,
                Phone = result.Data.Phone
            };

            return View(viewModel);
        }

        // POST: /Customers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")] 
        public async Task<IActionResult> Edit(EditCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var dto = new UpdateCustomerDto
            {
                Id = viewModel.Id,
                FullName = viewModel.FullName,
                Phone = viewModel.Phone
            };

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _customerService.UpdateAsync(dto, currentUserId);

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
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _customerService.DeleteAsync(id, currentUserId);

            if (!result.Success) TempData["Error"] = result.Message;
            else TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}