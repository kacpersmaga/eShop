using System.Net;
using System.Text.Json;
using eShop.Models.Domain;

namespace eShop.Extensions.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse 
            { 
                Error = "An unexpected error occurred. Please try again later."
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
