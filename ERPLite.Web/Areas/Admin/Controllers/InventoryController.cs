using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Interfaces.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;

        public InventoryController(
            IInventoryService inventoryService,
            IProductService productService)
        {
            _inventoryService = inventoryService;
            _productService = productService;
        }

        #region Low Stock
        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            var result = await _productService.GetLowStockProductsAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                return View(new List<ProductDto>());
            }

            return View(result.Data);
        }

        #endregion

        #region Stock In

        [HttpGet]
        public async Task<IActionResult> StockIn(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            var product =
                await _productService.GetByIdAsync(productId);

            if (!product.Success || product.Data is null)
            {
                TempData["Error"] = product.Message;

                return RedirectToAction(nameof(LowStock));
            }

            return View(new StockOperationDto
            {
                ProductId = productId,
                ProductName = product.Data.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockIn(
            StockOperationDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =
                await _inventoryService.StockInAsync(
                    model.ProductId,
                    model.Quantity,
                    model.Notes);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["Success"] = "Stock added successfully.";

            return RedirectToAction(nameof(History),
                new { productId = model.ProductId });
        }

        #endregion

        #region Stock Out

        [HttpGet]
        public async Task<IActionResult> StockOut(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            var product =
                await _productService.GetByIdAsync(productId);

            if (!product.Success || product.Data is null)
            {
                TempData["Error"] = product.Message;

                return RedirectToAction(nameof(LowStock));
            }

            return View(new StockOperationDto
            {
                ProductId = productId,
                ProductName = product.Data.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOut(
            StockOperationDto model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var result =
                await _inventoryService.StockOutAsync(
                    model.ProductId,
                    model.Quantity,
                    model.Notes);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["Success"] ="Stock deducted successfully.";

            return RedirectToAction(nameof(History),
                new { productId = model.ProductId });
        }

        #endregion

        #region Adjust

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Adjust(int productId)
        {
            if (productId <= 0)
                return BadRequest();

            var product =
                await _productService.GetByIdAsync(productId);

            if (!product.Success || product.Data is null)
            {
                TempData["Error"] = product.Message;

                return RedirectToAction(nameof(LowStock));
            }

            return View(new StockOperationDto
            {
                ProductId = productId,
                ProductName = product.Data.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Adjust(
            StockOperationDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =
                await _inventoryService.AdjustStockAsync(
                    model.ProductId,
                    model.Quantity,
                    model.Notes);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["Success"] =
                "Stock adjusted successfully.";

            return RedirectToAction(nameof(History),
                new { productId = model.ProductId });
        }

        #endregion

        #region History

        [HttpGet]
        public async Task<IActionResult> History(int productId,string? search)
        {
            if (productId <= 0)
                return BadRequest();

            var product =
                await _productService.GetByIdAsync(productId);

            if (!product.Success || product.Data is null)
            {
                TempData["Error"] = product.Message;

                return RedirectToAction(nameof(LowStock));
            }

            var result =
                await _inventoryService.GetHistoryAsync(productId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                return View(new List<StockMovementDto>());
            }
            var history = result.Data;
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search
                        .Trim()
                        .ToLower()
                        .Replace(" ", "");
                foreach (var item in history)
                {
                    Console.WriteLine(item.Type.ToString().Trim().ToLower());
                }
                history = history.Where(x =>
                    x.Type.ToString().Trim().ToLower().Contains(search)

                    || (!string.IsNullOrWhiteSpace(x.Notes)
                        && x.Notes.ToLower().Contains(search)));
            }

            ViewBag.Search = search;
            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.Data.Name;

            return View(history);
        }

        #endregion
    }
}
