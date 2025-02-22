using System.Net;
using eShop.Services.Interfaces;
using IntegrationTests.Fakes;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Extensions.Middleware;

public class ExceptionHandlingMiddlewareIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public ExceptionHandlingMiddlewareIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    

    [Fact]
    public async Task Middleware_Should_Not_Modify_Response_When_No_Exception()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/shop/items");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}