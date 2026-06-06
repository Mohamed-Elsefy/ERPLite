using ERPLite.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Payments
{
        public class CreatePaymentViewModel
        {
            [Required]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "Please specify the amount to be collected.")]
            [Range(0.01, double.MaxValue, ErrorMessage = "the amount must be greater than 0.")]
            [Display(Name = "Collection amount")]
            public decimal Amount { get; set; }

            [Required(ErrorMessage = "Please select a payment method.")]
            [Display(Name = "Payment method")]
            public PaymentMethod PaymentMethod { get; set; }

            public decimal RemainingBalance { get; set; }
        }
}
