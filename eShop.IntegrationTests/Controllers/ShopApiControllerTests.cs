using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using eShop.Data;
using eShop.Models.Dtos;
using eShop.Models.Domain;
using eShop.Services.Interfaces;
using IntegrationTests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using IntegrationTests.Fakes;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Controllers;

public class ShopApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ShopApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetItems_ReturnsListOfItems_IncludingNewlyAddedItem()
    {
        // Arrange
        var formData = new MultipartFormDataContent();
        var uniqueItemName = "Item for GET test " + Guid.NewGuid();
        formData.Add(new StringContent(uniqueItemName), "Name");
        formData.Add(new StringContent(9.99.ToString(CultureInfo.CurrentCulture)), "Price");
        formData.Add(new StringContent("Description for GET test"), "Description");
        formData.Add(new StringContent("GET Test Category"), "Category");
        var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        formData.Add(fileContent, "Image", "test.jpg");
        var addResponse = await _client.PostAsync("/api/admin/add", formData);
        if (!addResponse.IsSuccessStatusCode)
        {
            var error = await addResponse.Content.ReadAsStringAsync();
            throw new Exception($"Admin/AddItem failed: {error}");
        }

        // Act
        var response = await _client.GetAsync("/api/shop/items");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<ShopItemViewModel>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(items);
        Assert.True(items.Count > 0, "The list of items should not be empty.");
        Assert.Contains(items, item => item.Name == uniqueItemName);
    }
    
}
