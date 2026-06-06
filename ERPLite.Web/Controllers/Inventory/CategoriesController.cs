using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Web.Models.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.Inventory
{
    [Authorize(Policy = "ManagerOnly")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CategoriesController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var result = await _categoryService.GetAllAsync();
            var categories = result.Data ?? new List<CategoryDto>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                categories = categories.Where(c =>
                    c.Name.ToLower().Contains(cleanSearch) ||
                    (c.Description != null && c.Description.ToLower().Contains(cleanSearch))
                ).ToList();
            }

            var viewModel = new CategoryIndexViewModel
            {
                Categories = categories,
                SearchTerm = search
            };

            return View(viewModel);
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            var viewModel = new CategoryFormViewModel { IsEditMode = false };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CategoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new CreateCategoryDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            var result = await _categoryService.CreateAsync(dto, currentUserId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryFormViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                IsEditMode = true
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(CategoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = new UpdateCategoryDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            var result = await _categoryService.UpdateAsync(dto, currentUserId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _categoryService.DeleteAsync(id, currentUserId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            var productsResult = await _productService.GetProductsByCategoryIdAsync(id);
            var associatedProducts = productsResult.Data ?? new List<ProductDto>();

            var viewModel = new CategoryDetailsViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                AssociatedProducts = associatedProducts
            };

            return View(viewModel);
        }
    }
}
