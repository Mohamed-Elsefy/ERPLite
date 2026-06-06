using ERPLite.Data.Entities.Identity;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Web.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPLite.Web.Controllers.Sales
{
    [Authorize(Policy = "AllUsers")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(IPaymentService paymentService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: /Payments
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Index()
        {
            var result = await _paymentService.GetRecentPaymentsAsync(50);
            var data = result.Data ?? new List<PaymentDto>();

            var vm = data.Select(p => new PaymentsIndexViewModel
            {
                Id = p.Id,
                OrderId = 0,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                Status = p.Status
            }).ToList();

            return View(vm);
        }

        // GET: /Payments/OrderPayments/{orderId}
        public async Task<IActionResult> OrderPayments(int orderId)
        {
            var orderDetails = await _orderService.GetOrderDetailsAsync(orderId);
            if (!orderDetails.Success || orderDetails.Data == null)
            {
                TempData["Error"] = "Sales record target context not found.";
                return RedirectToAction("Index", "Orders");
            }

            var financialSummary = await _paymentService.GetOrderFinancialSummaryAsync(orderId);
            if (!financialSummary.Success || financialSummary.Data == null)
            {
                TempData["Error"] = "Unable to compute financial summary vectors.";
                return RedirectToAction("Index", "Orders");
            }

            var paymentsResult = await _paymentService.GetPaymentsByOrderAsync(orderId);
            var paymentsList = paymentsResult.Data ?? new List<PaymentDto>();

            var summaryData = financialSummary.Data;

            var vm = new OrderFinancialViewModel
            {
                OrderId = summaryData.OrderId,
                CustomerName = orderDetails.Data.CustomerName,
                TotalPrice = summaryData.TotalPrice,
                PaidAmount = summaryData.PaidAmount,
                RemainingAmount = summaryData.RemainingAmount,
                PaymentStatus = summaryData.PaymentStatus,
                Payments = paymentsList.Select(x => new PaymentItemViewModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    PaymentMethod = x.PaymentMethod,
                    PaymentDate = x.PaymentDate,
                    Status = x.Status
                }).ToList()
            };

            return View(vm);
        }

        // GET: /Payments/Create?orderId={id}
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Create(int orderId)
        {
            var summaryResult = await _paymentService.GetOrderFinancialSummaryAsync(orderId);
            if (!summaryResult.Success || summaryResult.Data == null)
            {
                TempData["Error"] = "Cannot initialize remittance voucher context.";
                return RedirectToAction("Index", "Orders");
            }

            var financial = summaryResult.Data;
            if (financial.RemainingAmount <= 0)
            {
                TempData["Error"] = "This ledger invoice is already fully cleared.";
                return RedirectToAction(nameof(OrderPayments), new { orderId });
            }

            var vm = new CreatePaymentViewModel
            {
                OrderId = orderId,
                Amount = financial.RemainingAmount,
                RemainingBalance = financial.RemainingAmount
            };

            return View(vm);
        }

        // POST: /Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Create(CreatePaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var balanceResult = await _paymentService.GetRemainingBalanceAsync(vm.OrderId);
                vm.RemainingBalance = balanceResult.Success ? balanceResult.Data : 0;
                return View(vm);
            }

            var dto = new CreatePaymentDto
            {
                OrderId = vm.OrderId,
                Amount = vm.Amount,
                PaymentMethod = vm.PaymentMethod
            };

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User session has expired. Please log in again.");
                var balanceResult = await _paymentService.GetRemainingBalanceAsync(vm.OrderId);
                vm.RemainingBalance = balanceResult.Success ? balanceResult.Data : 0;
                return View(vm);
            }

            var currentUserId = user.Id;

            var result = await _paymentService.CreatePaymentAsync(dto, currentUserId);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Remittance mapping failure.");

                var balanceResult = await _paymentService.GetRemainingBalanceAsync(vm.OrderId);
                vm.RemainingBalance = balanceResult.Success ? balanceResult.Data : 0;

                return View(vm);
            }

            TempData["Success"] = "Remittance transaction finalized and committed to sales ledger.";
            return RedirectToAction(nameof(OrderPayments), new { orderId = vm.OrderId });
        }
    }
}
