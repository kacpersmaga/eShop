using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using eShop.Data;
using eShop.Models.Domain;
using eShop.Services.Interfaces;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using IntegrationTests.Fakes;

namespace IntegrationTests.Controllers;

public class AdminControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AdminControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task AddItem_ValidModel_ReturnsOk()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("TestItem"), "Name");
        content.Add(new StringContent("49,99"), "Price");
        content.Add(new StringContent("TestCategory"), "Category");

        var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "Image", "test.jpg");

        // Act
        var response = await _client.PostAsync("/api/admin/add", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var item = context.ShopItems.FirstOrDefault(x => x.Name == "TestItem");

        Assert.NotNull(item);
        Assert.False(string.IsNullOrWhiteSpace(item.ImagePath), "Image path should not be null or empty");
        Assert.Matches(@"^[\w\d-]+\.jpg$", item.ImagePath);
    }

    [Fact]
    public async Task AddItem_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(""), "Name");
        content.Add(new StringContent("0"), "Price");
        content.Add(new StringContent(""), "Category");

        // Act
        var response = await _client.PostAsync("/api/admin/add", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
}
