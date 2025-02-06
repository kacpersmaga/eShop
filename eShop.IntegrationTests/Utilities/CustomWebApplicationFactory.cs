using Azure.Storage.Blobs;
using eShop;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Utilities;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureServices(services =>
        {
            string azuriteConnectionString = "UseDevelopmentStorage=true";
            services.AddSingleton(new BlobServiceClient(azuriteConnectionString));
        });
    }
}