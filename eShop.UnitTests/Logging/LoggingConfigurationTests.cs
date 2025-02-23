using eShop.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.TestCorrelator;

namespace UnitTests.Logging
{
    public class LoggingConfigurationTests
    {
        [Fact]
        public void ConfigureSerilog_ShouldInitializeLoggerAndWriteToConsole()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder();
            
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            // Act
            hostBuilder.ConfigureSerilog();

            // Assert
            Assert.NotNull(Log.Logger);

            using (TestCorrelator.CreateContext())
            {
                Log.Information("Test log message");

                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.Single(logEvents);
                Assert.Contains("Test log message", logEvents.First().RenderMessage());
            }
            
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        }
    }
}