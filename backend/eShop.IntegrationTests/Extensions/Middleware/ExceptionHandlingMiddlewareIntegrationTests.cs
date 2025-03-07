/*using System.Net;
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
    public async Task Middleware_Should_Catch_Exception_From_Controller_And_Return_500()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/shop/items");
        
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IItemService, FaultyItemService>();
            });
        }).CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("An unexpected error occurred", responseContent);
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
}*/