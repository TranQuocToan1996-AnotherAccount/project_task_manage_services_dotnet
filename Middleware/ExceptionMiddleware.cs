using System.Net;
using System.Text.Json;
using TaskManagement.Common;

namespace TaskManagement.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ApiResponse<object>();
        
        switch (exception)
        {
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = ApiResponse<object>.Fail(exception.Message);
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = ApiResponse<object>.Fail(exception.Message);
                break;
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.Fail(exception.Message);
                break;
            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.Fail(exception.Message);
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = ApiResponse<object>.Fail("An internal server error occurred. Please try again later.");
                break;
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
