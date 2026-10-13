using ERPLite.Web.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Common
{
    /// <summary>
    /// Friendly error page. Target of ExceptionHandlingMiddleware redirects
    /// (non-API requests) and UseExceptionHandler("/Error") in production.
    /// </summary>
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: /Error?code=500&requestId=...
        [HttpGet("/Error")]
        public IActionResult Index(int code = 500, string? requestId = null)
        {
            Response.StatusCode = code;
            return View("~/Views/Shared/Error/Error.cshtml", new ErrorViewModel
            {
                RequestId = requestId ?? HttpContext.TraceIdentifier
            });
        }
    }
}
