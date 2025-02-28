/*
using eShop.Data;
using eShop.Extensions.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedTestUtilities.Fakes;

namespace UnitTests.Extensions.Database;

public class DatabaseConfigurationTests
{
    [Fact]
    public void ConfigureDatabase_ShouldUseTestConnectionString_WhenEnvironmentIsTest()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:TestConnection", "TestConnectionString" },
                { "ConnectionStrings:DefaultConnection", "DefaultConnectionString" }
            })
            .Build();

        var env = new FakeHostEnvironment { EnvironmentName = "Test" };

        // Act
        services.ConfigureDatabase(configuration, env);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
        Assert.NotNull(dbContextOptions);
    }

    [Fact]
    public void ConfigureDatabase_ShouldUseDefaultConnectionString_WhenEnvironmentIsNotTest()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "DefaultConnectionString" }
            })
            .Build();

        var env = new FakeHostEnvironment { EnvironmentName = "Development" };

        // Act
        services.ConfigureDatabase(configuration, env);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
        Assert.NotNull(dbContextOptions);
    }

    [Fact]
    public void ConfigureDatabase_ShouldThrowException_WhenConnectionStringIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        var env = new FakeHostEnvironment { EnvironmentName = "Development" };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.ConfigureDatabase(configuration, env));
        Assert.Equal("Database connection string is not configured.", exception.Message);
    }
    
}
*/
