using Azure.Storage.Blobs;
using DotNet.Testcontainers.Builders;
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
    private static readonly MsSqlContainer DbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .WithPassword("YourStrong!Passw0rd")
        .Build();

    private static readonly AzuriteContainer AzuriteContainer = new AzuriteBuilder()
        .WithCommand("--skipApiVersionCheck")
        .Build();
    private static BlobServiceClient? _sharedBlobServiceClient;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(DbContainer.GetConnectionString()));
            
            services.RemoveAll<BlobServiceClient>();
            
            services.AddSingleton<BlobServiceClient>(_ =>
                _sharedBlobServiceClient ??= new BlobServiceClient(AzuriteContainer.GetConnectionString()));
            
            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();
        });
    }

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();
        await AzuriteContainer.StartAsync();

        _sharedBlobServiceClient ??= new BlobServiceClient(AzuriteContainer.GetConnectionString());

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var blobClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var imagesContainer = blobClient.GetBlobContainerClient("images");
        await imagesContainer.CreateIfNotExistsAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {

    }
}
