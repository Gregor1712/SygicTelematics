using System.Net;
using System.Text.Json;
using TestProject.Errors;

namespace TestProject.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, ILogger<ExceptionMiddleware> logger, RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (!context.Response.HasStarted && context.Response.StatusCode >= 400 && context.Response.ContentLength is null)
            {
                await HandleStatusCodeAsync(context);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleStatusCodeAsync(HttpContext context)
    {
        var message = context.Response.StatusCode switch
        {
            401 => "You are not authorized",
            403 => "You do not have permission to access this resource",
            404 => "Resource not found",
            _   => "An error occurred"
        };

        logger.LogError(context.Response.StatusCode, "An exception occurred: {Message}", message);
        
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse(context.Response.StatusCode, message, null);
        await context.Response.WriteAsJsonAsync(response, JsonOptions);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ArgumentException    => (HttpStatusCode.BadRequest, ex.Message),
            _                    => (HttpStatusCode.InternalServerError, "Internal server error")
        };

        logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = env.IsDevelopment()
            ? new ApiErrorResponse((int)statusCode, ex.Message, ex.StackTrace)
            : new ApiErrorResponse((int)statusCode, message, null);

        await context.Response.WriteAsJsonAsync(response, JsonOptions);
    }
}