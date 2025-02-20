using Azure.Storage.Blobs;
using eShop.Services.Interfaces;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Extensions.Storage;

public class BlobStorageConfigurationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public void Application_ShouldRegisterBlobStorageServices()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        // Act & Assert
        Assert.NotNull(provider.GetService<BlobServiceClient>());
        Assert.NotNull(provider.GetService<IBlobStorageService>());
        Assert.NotNull(provider.GetService<IBlobStorageServiceWrapper>());
    }
}