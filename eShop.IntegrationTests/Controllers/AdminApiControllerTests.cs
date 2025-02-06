using System.Net;
using System.Net.Http.Headers;
using eShop.Data;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Controllers;

public class AdminControllerIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

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
        
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var item = context.ShopItems.FirstOrDefault(x => x.Name == "TestItem");

        Assert.NotNull(item);
        Assert.False(string.IsNullOrWhiteSpace(item.ImagePath), "Image path should not be null or empty");
        
        Assert.Matches(@"^[\w\d-]+\.jpg$", item.ImagePath);
    }

    [Fact]
    public async Task AddItem_InvalidModel_ReturnsBadRequest()
    {
        // This test requires Azurite to be running for Blob Storage emulation.
        // If Azurite is not running, the test will fail with a 500 Internal Server Error.
        // If Azurite is running, the test will fail with a 400 Bad Request.
        
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(""), "Name");
        content.Add(new StringContent("0"), "Price");
        content.Add(new StringContent(""), "Category");
        var response = await _client.PostAsync("/api/admin/add", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /*
    [Fact]
    public async Task AddItem_AzuriteUnavailable_ReturnsServiceUnavailable()
    {
        //    The unit test "AddItem_ExceptionThrown_ReturnsInternalServerError" ensures that:
        //    1. The controller correctly catches exceptions.
        //    2. A proper 500 Internal Server Error response is returned.
        //    3. Logging works as expected.
        //    If we still want to test Azurite failures, we should stop Azurite before running this test.

        // Arrange
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("ErrorItem"), "Name");
        content.Add(new StringContent("99,99"), "Price");
        content.Add(new StringContent("ErrorCategory"), "Category");

        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "Image", "test.jpg");

        // Act
        var response = await _client.PostAsync("/api/Admin/AddItem", content);

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
    */
}

