/*
using Azure.Storage.Blobs;
using eShop;
using eShop.Data;
using eShop.Services.Implementations;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace IntegrationTests.Utilities;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .WithPassword("YourStrong!Passw0rd")
        .Build();

    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithCommand("--skipApiVersionCheck")
        .Build();
        
    private BlobServiceClient? _blobServiceClient;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));
            
            services.RemoveAll<BlobServiceClient>();
            
            services.AddSingleton<BlobServiceClient>(_ =>
                _blobServiceClient ??= new BlobServiceClient(_azuriteContainer.GetConnectionString()));
            
            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _azuriteContainer.StartAsync();

        _blobServiceClient = new BlobServiceClient(_azuriteContainer.GetConnectionString());

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var blobClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var imagesContainer = blobClient.GetBlobContainerClient("images");
        await imagesContainer.CreateIfNotExistsAsync();
    }

    public new Task DisposeAsync()
    {
        // Containers doesn't need disposing, as it doesn't cause conflicts and it speeds up testing
        return Task.CompletedTask; 
    }
}
*/

