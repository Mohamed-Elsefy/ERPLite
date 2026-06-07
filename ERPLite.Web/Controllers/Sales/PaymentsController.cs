using AutoMapper;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPLite.Web.Controllers.Sales
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public PaymentsController(
            IPaymentService paymentService,
            IOrderService orderService,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // GET: /Payments
        public async Task<IActionResult> Index()
        {
            var result = await _paymentService.GetRecentPaymentsAsync(50);
            var data = result.Data ?? new List<PaymentDto>();

            var vm = _mapper.Map<List<PaymentsIndexViewModel>>(data);
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
                Payments = _mapper.Map<List<PaymentItemViewModel>>(paymentsList)
            };

            return View(vm);
        }

        // GET: /Payments/Create?orderId={id}
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
        public async Task<IActionResult> Create(CreatePaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var balanceResult = await _paymentService.GetRemainingBalanceAsync(vm.OrderId);
                vm.RemainingBalance = balanceResult.Success ? balanceResult.Data : 0;
                return View(vm);
            }

            var dto = _mapper.Map<CreatePaymentDto>(vm);
            var currentUserId = _currentUser.UserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                ModelState.AddModelError(string.Empty, "User session has expired. Please log in again.");
                var balanceResult = await _paymentService.GetRemainingBalanceAsync(vm.OrderId);
                vm.RemainingBalance = balanceResult.Success ? balanceResult.Data : 0;
                return View(vm);
            }

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