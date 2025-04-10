using System.Net;
using System.Text.Json;
using eShop.Shared.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred. Please try again later.";
        
        var exceptionType = exception.GetType().FullName;
        
        if (exceptionType?.Contains(".Catalog.Domain.Exceptions.") == true)
        {
            if (exceptionType.EndsWith("ProductNotFoundException"))
            {
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exceptionType.EndsWith("ProductAlreadyExistsException"))
            {
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
            }
            else if (exceptionType.EndsWith("InvalidProductDataException"))
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exceptionType.EndsWith("ProductDomainException"))
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
        }
        else if (exception is ArgumentException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message;
        }
        
        var response = ApiResponse<object>.CreateFailure(message, (int)statusCode);
        
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        }));
    }
}