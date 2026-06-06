using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.ViewComponents
{
    public class NotificationDropdownViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Mock data for notification dropdown
            var unreadCount = 3;
            return View(unreadCount);
        }
    }
}
