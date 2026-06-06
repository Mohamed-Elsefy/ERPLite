using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Employee.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(
            ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var result = await _supplierService.GetAllAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                return View(new List<SupplierDto>());
            }

            var suppliers = result.Data;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                suppliers = suppliers.Where(x =>
                    x.Name.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    || x.Phone.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    || (!string.IsNullOrWhiteSpace(x.Address)
                        && x.Address.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)));
            }

            ViewBag.Search = search;

            return View(suppliers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result = await _supplierService.GetByIdAsync(id);

            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;

                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateSupplierDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =
                await _supplierService.CreateAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["Success"] =
                "Supplier created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result =
                await _supplierService.GetByIdAsync(id);

            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;

                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateSupplierDto
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Phone = result.Data.Phone,
                Address = result.Data.Address,
                Email = result.Data.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            UpdateSupplierDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =
                await _supplierService.UpdateAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["Success"] =
                "Supplier updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var result =
                await _supplierService.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] =
                    "Supplier deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
