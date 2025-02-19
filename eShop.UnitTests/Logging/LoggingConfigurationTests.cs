using eShop.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace UnitTests.Logging;

public class LoggingConfigurationTests
{
    [Fact]
    public void ConfigureSerilog_ShouldConfigureLoggerWithoutThrowing()
    {
        // Arrange
        var builder = Host.CreateDefaultBuilder();

        // Act
        var exception = Record.Exception(() => builder.ConfigureSerilog());

        // Assert
        Assert.Null(exception);
    }
}