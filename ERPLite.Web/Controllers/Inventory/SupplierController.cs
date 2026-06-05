using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Inventory
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
