using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Web.Models.Suppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.Inventory
{
    [Authorize(Policy = "ManagerOnly")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;

        public SuppliersController(ISupplierService supplierService, IProductService productService)
        {
            _supplierService = supplierService;
            _productService = productService;
        }

        // GET: /Suppliers
        public async Task<IActionResult> Index(string search)
        {
            var result = await _supplierService.GetAllAsync();
            var suppliers = result.Data ?? new List<SupplierDto>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                suppliers = suppliers.Where(s =>
                    s.Name.ToLower().Contains(cleanSearch) ||
                    s.Phone.Contains(cleanSearch) ||
                    (s.Address != null && s.Address.ToLower().Contains(cleanSearch))
                ).ToList();
            }

            var viewModel = new SupplierIndexViewModel
            {
                Suppliers = suppliers,
                SearchTerm = search
            };

            return View(viewModel);
        }

        // GET: /Suppliers/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _supplierService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Supplier not found";
                return RedirectToAction(nameof(Index));
            }

            var productsResult = await _productService.GetProductsBySupplierIdAsync(id);
            var suppliedProducts = productsResult.Data ?? new List<ProductDto>();

            var viewModel = new SupplierDetailsViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Phone = result.Data.Phone,
                Address = result.Data.Address,
                SuppliedProducts = suppliedProducts
            };

            return View(viewModel);
        }

        #region Admin Only - Supply Chain Management

        // GET: /Suppliers/Create
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            var viewModel = new SupplierFormViewModel { IsEditMode = false };
            return View(viewModel);
        }

        // POST: /Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(SupplierFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new CreateSupplierDto
            {
                Name = viewModel.Name,
                Phone = viewModel.Phone,
                Address = viewModel.Address
            };

            var result = await _supplierService.CreateAsync(dto, currentUserId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Suppliers/Edit/{id}
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _supplierService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Supplier not found";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SupplierFormViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Phone = result.Data.Phone,
                Address = result.Data.Address,
                IsEditMode = true
            };

            return View(viewModel);
        }

        // POST: /Suppliers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(SupplierFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new UpdateSupplierDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Phone = viewModel.Phone,
                Address = viewModel.Address
            };

            var result = await _supplierService.UpdateAsync(dto, currentUserId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Suppliers/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _supplierService.DeleteAsync(id, currentUserId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
