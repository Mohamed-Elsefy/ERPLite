using ERPLite.Shared.Enums;

namespace ERPLite.Web.Helpers
{
    public static class PaymentStatusHelper
    {
        public static string GetBadgeClass(OrderPaymentStatus status)
        {
            return status switch
            {
                OrderPaymentStatus.Paid => "success",
                OrderPaymentStatus.PartiallyPaid => "warning",
                OrderPaymentStatus.Unpaid => "danger",
                _ => "secondary"
            };
        }
    }
}
