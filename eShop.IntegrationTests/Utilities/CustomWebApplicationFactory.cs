using Azure.Storage.Blobs;
using DotNet.Testcontainers.Builders;
using eShop;
using eShop.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace IntegrationTests.Utilities
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private static readonly MsSqlContainer DbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPassword("YourStrong!Passw0rd")
            .WithPortBinding(11433, 1433)
            .WithWaitStrategy(Wait.ForUnixContainer()
            )
            .Build();
        
        private static readonly AzuriteContainer AzuriteContainer = new AzuriteBuilder()
            .WithCommand("--skipApiVersionCheck")
            .WithPortBinding(10000, 10000)
            .WithPortBinding(10001, 10001)
            .WithPortBinding(10002, 10002)
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
                {
                    return _sharedBlobServiceClient ??=
                        new BlobServiceClient(AzuriteContainer.GetConnectionString());
                });
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

        public async Task DisposeAsync()
        {
            await DbContainer.StopAsync();
            await AzuriteContainer.StopAsync();
        }
    }
}