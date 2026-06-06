using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        [HttpGet("unread")]
        public IActionResult GetUnread()
        {
            return Ok(new { Count = 3 });
        }

        [HttpPost("mark-as-read")]
        public IActionResult MarkAsRead()
        {
            return Ok(new { Success = true });
        }

        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            return Ok(new[]
            {
                new { Id = 1, Title = "Stock Alert", Message = "Product is low", Priority = "High" }
            });
        }
    }
}
