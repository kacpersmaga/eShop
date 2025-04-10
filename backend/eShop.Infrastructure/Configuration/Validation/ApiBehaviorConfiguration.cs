using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Configuration.Validation;

public static class ApiBehaviorConfiguration
{
    public static IServiceCollection AddApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiBehaviorOptions>>();
                logger.LogWarning("Validation failed: {Errors}", string.Join("; ",
                    context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

                return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            };
        });

        return services;
    }
}