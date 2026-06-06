using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Web.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.Inventory
{
    [Authorize(Policy = "ManagerOnly")] 
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
        }

        // GET: /Products
        public async Task<IActionResult> Index(string search)
        {
            var result = await _productService.GetAllAsync();
            var products = result.Data ?? new List<ProductDto>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(cleanSearch) ||
                    p.CategoryName.ToLower().Contains(cleanSearch) ||
                    p.SupplierName.ToLower().Contains(cleanSearch)
                ).ToList();
            }

            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                SearchTerm = search
            };

            return View(viewModel);
        }

        // GET: /Products/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Product could not be resolved.";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        #region Admin Only - Procurement & Structuring Control

        // GET: /Products/Create
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductFormViewModel { IsEditMode = false };
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(ProductFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new CreateProductDto
            {
                Name = viewModel.Name,
                Price = viewModel.Price,
                QuantityInStock = viewModel.QuantityInStock,
                MinStockLevel = viewModel.MinStockLevel,
                CategoryId = viewModel.CategoryId,
                SupplierId = viewModel.SupplierId
            };

            var result = await _productService.CreateAsync(dto, currentUserId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Edit/{id}
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Product entity not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ProductFormViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Price = result.Data.Price,
                QuantityInStock = result.Data.QuantityInStock,
                MinStockLevel = result.Data.MinStockLevel,
                CategoryId = result.Data.CategoryId,
                SupplierId = result.Data.SupplierId,
                IsEditMode = true
            };

            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }

        // POST: /Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(ProductFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new UpdateProductDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Price = viewModel.Price,
                QuantityInStock = viewModel.QuantityInStock,
                MinStockLevel = viewModel.MinStockLevel,
                CategoryId = viewModel.CategoryId,
                SupplierId = viewModel.SupplierId
            };

            var result = await _productService.UpdateAsync(dto, currentUserId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Products/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _productService.DeleteAsync(id, currentUserId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        #endregion

        private async Task PopulateDropdownsAsync(ProductFormViewModel viewModel)
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            var suppliersResult = await _supplierService.GetAllAsync();

            viewModel.CategoriesList = (categoriesResult.Data ?? new List<CategoryDto>())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });

            viewModel.SuppliersList = (suppliersResult.Data ?? new List<SupplierDto>())
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name });
        }
    }
}
