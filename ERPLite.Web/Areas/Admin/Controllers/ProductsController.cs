using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Shared.Constants;
using ERPLite.Web.Areas.Admin.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index(string? search)
        {
            var result = await _productService.GetAllAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                return View(new List<ProductDto>());
            }

            var products = result.Data;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                products = products.Where(p =>
                    p.Name.Contains(search,
                        StringComparison.OrdinalIgnoreCase)

                    || p.SKU.Contains(search,
                        StringComparison.OrdinalIgnoreCase)

                    || p.CategoryName.Contains(search,
                        StringComparison.OrdinalIgnoreCase)

                    || p.SupplierName.Contains(search,
                        StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Search = search;

            return View(products);
        }
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result = await _productService.GetByIdAsync(id);

            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;

                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Create()
        {
          var vm =  await LoadDropdowns();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Create(
            ProductFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
              var dropdowns =  await LoadDropdowns();
                vm.Categories = dropdowns.Categories;
                vm.Suppliers = dropdowns.Suppliers;

                return View(vm);
            }

            var result =
                await _productService.CreateAsync(vm.Product);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                var dropdowns = await LoadDropdowns();
                vm.Categories = dropdowns.Categories;
                vm.Suppliers = dropdowns.Suppliers;

                return View(vm);
            }

            TempData["Success"] =
                "Product created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();
            var result =
                await _productService.GetByIdAsync(id);

            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;

                return RedirectToAction(nameof(Index));
            }

            var vm = new UpdateProducFormVm
            {
                Product = new UpdateProductDto
                {
                    Id = result.Data.Id,
                    Name = result.Data.Name,
                    SKU = result.Data.SKU,
                    CostPrice = result.Data.CostPrice,
                    SellingPrice = result.Data.SellingPrice,
                    QuantityInStock = result.Data.QuantityInStock,
                    MinStockLevel = result.Data.MinStockLevel,
                    CategoryId = result.Data.CategoryId,
                    SupplierId = result.Data.SupplierId
                }
            };

            var dropdowns = await LoadDropdowns();

            vm.Categories = dropdowns.Categories;
            vm.Suppliers = dropdowns.Suppliers;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Edit(
            UpdateProducFormVm vm)
        {

            if (!ModelState.IsValid)
            {
                var dropdowns = await LoadDropdowns();

                vm.Categories = dropdowns.Categories;
                vm.Suppliers = dropdowns.Suppliers;

                return View(vm);
            }

            var result =
                await _productService.UpdateAsync(vm.Product);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                var dropdowns = await LoadDropdowns();

                vm.Categories = dropdowns.Categories;
                vm.Suppliers = dropdowns.Suppliers;

                return View(vm);
            }

            TempData["Success"] =
                "Product updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result =
                await _productService.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] =
                    "Product deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Employee")]

        public async Task<IActionResult> LowStock()
        {
            var result =
                await _productService.GetLowStockProductsAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                return View(new List<ProductDto>());
            }

            return View(result.Data);
        }

        private async Task<ProductFormViewModel> LoadDropdowns()
        {
            var categories =
                await _categoryService.GetAllAsync();

            var suppliers =
                await _supplierService.GetAllAsync();

            return new ProductFormViewModel
            {
                Categories = categories.Data!.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }),

                Suppliers = suppliers.Data!.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
            };

        }

}

}
