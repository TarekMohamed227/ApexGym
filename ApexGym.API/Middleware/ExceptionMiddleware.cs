using ApexGym.Domain.Common;
using System.Net;
using System.Text.Json;

namespace ApexGym.API.Middleware
{
    // This class will be our custom middleware for handling exceptions
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // If no exception happens, just continue processing the request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the error with context
                _logger.LogError(ex, "An unhandled exception occurred. Request Path: {RequestPath}", context.Request.Path);
                context.Response.ContentType = "application/json";

                // ===== NEW & SMARTER STATUS CODE LOGIC =====
                // This checks the type of the exception that was caught.
                context.Response.StatusCode = ex switch
                {
                    // Pattern: [Type to check] => [Value to return if it matches]
                    NotFoundException => StatusCodes.Status404NotFound, // If it's a NotFoundException, return 404
                                                                        // We can add more lines here later for other custom exceptions...
                                                                        // For example:
                                                                        // BadRequestException => StatusCodes.Status400BadRequest,

                    // The underscore '_' is the default case. If it's any other type of exception, return 500.
                    _ => StatusCodes.Status500InternalServerError
                };
                // ===========================================

                // ... rest of the code (creating the response, converting to JSON) stays the same ...
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, ex switch
                    {
                        NotFoundException => ex.Message,
                        _ => "An internal server error has occurred."
                    });

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }

    // A standard class to structure our error response
    public class ApiException
    {
        public ApiException(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; } // Stack trace or other details, only for development
    }
}
