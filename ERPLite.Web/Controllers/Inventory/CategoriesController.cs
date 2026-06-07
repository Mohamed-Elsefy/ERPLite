using AutoMapper;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Models.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Inventory
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, ICurrentUserService currentUser, IMapper mapper)
        {
            _categoryService = categoryService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // GET: /Categories
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

        // GET: /Categories/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<CategoryDetailsViewModel>(result.Data);
            return View(viewModel);
        }

        #region Admin Only Actions

        // GET: /Categories/Create
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel { IsEditMode = false });
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CategoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = new CreateCategoryDto { Name = viewModel.Name, Description = viewModel.Description };
            var result = await _categoryService.CreateAsync(dto, _currentUser.UserId!);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/{id}
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

        // POST: /Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")] 
        public async Task<IActionResult> Edit(CategoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = new UpdateCategoryDto { Id = viewModel.Id, Name = viewModel.Name, Description = viewModel.Description };
            var result = await _categoryService.UpdateAsync(dto, _currentUser.UserId!);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Categories/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id, _currentUser.UserId!);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}