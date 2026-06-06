using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Shared.Helpers;
using ERPLite.Web.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPLite.Web.Controllers.Sales
{
    [Authorize(Policy = "AllUsers")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public OrdersController(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var result = await _orderService.GetRecentOrdersAsync();
            var orders = result.Data ?? new List<OrderDto>();
            return View(orders);
        }

        // GET: /Orders/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _orderService.GetOrderDetailsAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: /Orders/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateOrderViewModel();
            await LoadDropdownsIntoViewModelAsync(viewModel);
            return View(viewModel);
        }

        // POST: /Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            if (viewModel.Items == null || !viewModel.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "Should include at least one item.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsIntoViewModelAsync(viewModel);
                return View(viewModel);
            }

            var dto = new CreateOrderDto
            {
                CustomerId = viewModel.CustomerId,
                CreatedByUserId = User.GetUserId() ?? "System",
                Items = viewModel.Items!.Select(x => new CreateOrderItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()
            };

            var currentUserId = User.GetUserId() ?? "System";
            var result = await _orderService.CreateOrderAsync(dto, currentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                await LoadDropdownsIntoViewModelAsync(viewModel);
                return View(viewModel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsIntoViewModelAsync(CreateOrderViewModel viewModel)
        {
            var customersResult = await _customerService.GetAllAsync();
            var productsResult = await _productService.GetAllAsync();

            var activeCustomers = customersResult.Data ?? new List<CustomerDto>();
            var activeProducts = productsResult.Data ?? new List<ProductDto>();

            viewModel.Customers = activeCustomers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName 
            }).ToList();

            viewModel.Products = activeProducts.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} (Price: {p.Price:C2} | Stock: {p.QuantityInStock})"
            }).ToList();

            ViewBag.ProductsRaw = activeProducts.Select(p => new { p.Id, p.Price, p.Name }).ToList();
        }
    }
}
