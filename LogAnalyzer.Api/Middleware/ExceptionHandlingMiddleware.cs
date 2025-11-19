using LogAnalyzer.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace LogAnalyzer.Api.Middleware
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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // Log non-success status codes globally
                if (context.Response.StatusCode >= 400)
                {
                    _logger.LogWarning("Request {method} {url} returned status {statusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(exception,
                "Unhandled exception. TraceId: {traceId}, Path: {path}",
                traceId, context.Request.Path);

            var error = new ApiError
            {
                Message = "An unexpected error occurred.",
                Detail = exception.Message,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                TraceId = traceId
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.StatusCode;

            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
