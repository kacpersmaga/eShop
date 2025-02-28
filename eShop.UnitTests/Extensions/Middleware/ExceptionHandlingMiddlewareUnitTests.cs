/*using System.Net;
using System.Text.Json;
using eShop.Extensions.Middlewares;
using eShop.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Extensions.Middleware;

public class ExceptionHandlingMiddlewareUnitTests
{
    [Fact]
    public async Task Invoke_Should_Return_500_And_Json_On_Exception()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var middleware = new ExceptionHandlingMiddleware(
            next: (_) => throw new Exception("Test exception"),
            loggerMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        Assert.NotNull(errorResponse);
        Assert.Equal("An unexpected error occurred. Please try again later.", errorResponse.Error);
        
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception occurred.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Invoke_Should_Not_Modify_Response_When_No_Exception()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        var middleware = new ExceptionHandlingMiddleware(
            next: (_) => Task.CompletedTask,
            loggerMock.Object
        );

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Null(context.Response.ContentType);
        
        loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never
        );
    }
}*/