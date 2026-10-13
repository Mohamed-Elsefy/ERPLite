using ERPLite.Web.Models.Common;

namespace ERPLite.Web.Models.Payments
{
    public class PaymentsListViewModel
    {
        public IEnumerable<PaymentsIndexViewModel> Payments { get; set; } = new List<PaymentsIndexViewModel>();
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
