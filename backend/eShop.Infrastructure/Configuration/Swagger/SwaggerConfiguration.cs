using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace eShop.Infrastructure.Configuration.Swagger;

public static class SwaggerConfiguration
{
    private const string ApiTitle = "eShop API";
    private const string ApiVersion = "v1";
    private const string SwaggerEndpoint = "/swagger/v1/swagger.json";

    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(ApiVersion, new OpenApiInfo 
            { 
                Title = ApiTitle, 
                Version = ApiVersion,
                Description = "REST API for the eShop application",
                Contact = new OpenApiContact
                {
                    Name = "eShop Development Team",
                    Email = "team@eshop.com"
                }
            });
            
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            foreach (var xmlFile in xmlFiles)
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }
            
            options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            
            options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsStaging())
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });
            
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(SwaggerEndpoint, $"{ApiTitle} {ApiVersion}");
                options.RoutePrefix = "api-docs";
                options.DocumentTitle = ApiTitle;
                options.DefaultModelsExpandDepth(-1);
                options.DisplayRequestDuration();
            });
        }

        return app;
    }
}