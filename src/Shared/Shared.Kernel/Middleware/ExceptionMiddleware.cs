using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Kernel.Errors;

namespace Shared.Kernel.Middleware;

public class ExceptionMiddleware(
    IHostEnvironment env,
    ILogger<ExceptionMiddleware> logger,
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (!context.Response.HasStarted && context.Response.Body == Stream.Null
                || context.Response.ContentLength == 0)
            {
                var statusCode = context.Response.StatusCode;
                var message = statusCode switch
                {
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "Not Found",
                    _ => null
                };

                if (message != null)
                {
                    context.Response.ContentType = "application/json";
                    var response = new ApiErrorResponse(statusCode, message);
                    var json = JsonSerializer.Serialize(response, JsonOptions);
                    await context.Response.WriteAsync(json);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var errorResponse = new ApiErrorResponse(
                context.Response.StatusCode,
                env.IsDevelopment() ? ex.Message : "An error occurred",
                env.IsDevelopment() ? ex.StackTrace : null);

            var errorJson = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await context.Response.WriteAsync(errorJson);
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}