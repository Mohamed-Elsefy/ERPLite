using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index(string? search)
        {
            var result =
                string.IsNullOrWhiteSpace(search)
                ? await _categoryService.GetAllAsync()
                : await _categoryService.SearchAsync(search);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                return View(new List<CategoryDto>());
            }

            return View(result.Data);
        }
        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }
        [HttpGet]
        public IActionResult Create() { return View(); }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            if (!ModelState.IsValid)
                return View(model); 
            var result = await _categoryService.CreateAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet] 
        public async Task<IActionResult> Edit(int id) 
        { 
            var result = await _categoryService.GetByIdAsync(id); 
            if (!result.Success || result.Data is null) 
            { 
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            } 
            var model = new UpdateCategoryDto {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(UpdateCategoryDto model) 
        { 
            if (!ModelState.IsValid)
                return View(model); 
            var result = await _categoryService.UpdateAsync(model);
            if (!result.Success) 
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            } 
            TempData["Success"] = "Category updated successfully."; 
            return RedirectToAction(nameof(Index)); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message; 
                return RedirectToAction(nameof(Index));
            } 
            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }

}
