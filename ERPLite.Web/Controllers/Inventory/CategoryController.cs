using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Inventory
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
