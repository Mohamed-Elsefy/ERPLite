using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Orders
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Select a customer")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new();

        public decimal GrandTotal { get; set; }

        public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> Products { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
