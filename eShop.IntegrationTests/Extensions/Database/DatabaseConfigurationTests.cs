using eShop.Data;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Extensions.Database;

public class DatabaseConfigurationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ApplicationDbContext_ShouldConnectToDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        Assert.True(canConnect);
    }
}

