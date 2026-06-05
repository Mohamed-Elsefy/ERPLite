using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Inventory
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
