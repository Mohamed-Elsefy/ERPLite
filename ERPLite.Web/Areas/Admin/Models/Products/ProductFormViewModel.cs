using ERPLite.Services.DTOs.Inventory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPLite.Web.Areas.Admin.Models.Products
{
    public class ProductFormViewModel
    {
        public CreateProductDto Product { get; set; } = new();

        public IEnumerable<SelectListItem> Categories { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> Suppliers { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
