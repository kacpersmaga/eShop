using eShop.Data;
using IntegrationTests.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Extensions.Database;

public class DatabaseConfigurationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public void ConfigureDatabase_ShouldConfigureDbContextWithTestConnectionString()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Act
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var connectionString = dbContext.Database.GetDbConnection().ConnectionString;

        // Assert
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var expectedConnectionString = configuration.GetConnectionString("TestConnection");

        Assert.Equal(expectedConnectionString, connectionString);
    }
}

