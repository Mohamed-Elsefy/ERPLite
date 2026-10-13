using ERPLite.Services.Exceptions;
using ERPLite.Web.Models.Common;
using System.Net;
using System.Text.Json;

namespace ERPLite.Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception {ExceptionType} on {Method} {Path} (TraceId: {TraceId})",
                    ex.GetType().Name, context.Request.Method, context.Request.Path, context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                // Too late to produce a friendly response; already logged above.
                return Task.CompletedTask;
            }

            var (statusCode, userMessage, details) = MapException(exception);

            // API / AJAX callers expect JSON; full-page requests get the friendly error page.
            if (WantsJson(context))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var response = new ExceptionResponse
                {
                    StatusCode = statusCode,
                    Message = userMessage,
                    Details = details
                };

                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }

            context.Response.Redirect($"/Error?code={statusCode}&requestId={context.TraceIdentifier}");
            return Task.CompletedTask;
        }

        private static (int StatusCode, string Message, string? Details) MapException(Exception exception)
        {
            return exception switch
            {
                NotFoundException notFoundEx => ((int)HttpStatusCode.NotFound, notFoundEx.Message, null),
                BusinessException businessEx => ((int)HttpStatusCode.BadRequest, businessEx.Message, null),
                ValidationException validationEx => (
                    (int)HttpStatusCode.BadRequest,
                    "Validation failed.",
                    string.Join(Environment.NewLine, validationEx.Errors)),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.", null),
            };
        }

        private static bool WantsJson(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(context.Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                return true;

            return context.Request.Headers.Accept.ToString()
                .Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
