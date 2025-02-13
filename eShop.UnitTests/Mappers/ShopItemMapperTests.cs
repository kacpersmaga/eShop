using AutoMapper;
using eShop.Mappers.Profiles;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using UnitTests.Fakes;

namespace UnitTests.Mappers;
public class ShopItemMappingProfileTests
{
    private readonly IMapper _mapper;

    public ShopItemMappingProfileTests()
    {
        var fakeImageService = new FakeImageService();
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ShopItemMappingProfile());
            cfg.ConstructServicesUsing(type => type == typeof(ImageUriResolver) 
                ? new ImageUriResolver(fakeImageService) 
                : null);
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void ShopItemFormModel_To_ShopItem_Mapping_IsValid()
    {
        // Arrange
        var formModel = new ShopItemFormModel
        {
            Name = "Test Item",
            Price = 99.99m,
            Description = "Test Description",
            Category = "Test Category",
            Image = null
        };

        // Act
        var shopItem = _mapper.Map<ShopItem>(formModel);

        // Assert
        Assert.NotNull(shopItem);
        Assert.Equal(formModel.Name, shopItem.Name);
        Assert.Equal(formModel.Price, shopItem.Price);
        Assert.Equal(formModel.Description, shopItem.Description);
        Assert.Equal(formModel.Category, shopItem.Category);
        Assert.Null(shopItem.ImagePath);
    }

    [Fact]
    public void ShopItem_To_ShopItemFormModel_Mapping_IsValid()
    {
        // Arrange
        var shopItem = new ShopItem
        {
            Id = 1,
            Name = "Test Item",
            Price = 99.99m,
            Description = "Test Description",
            Category = "Test Category",
            ImagePath = "test-image.jpg"
        };

        // Act
        var formModel = _mapper.Map<ShopItemFormModel>(shopItem);

        // Assert
        Assert.NotNull(formModel);
        Assert.Equal(shopItem.Name, formModel.Name);
        Assert.Equal(shopItem.Price, formModel.Price);
        Assert.Equal(shopItem.Description, formModel.Description);
        Assert.Equal(shopItem.Category, formModel.Category);
    }

    [Fact]
    public void ShopItem_To_ShopItemViewModel_Mapping_IsValid()
    {
        // Arrange
        var shopItem = new ShopItem
        {
            Id = 1,
            Name = "Test Item",
            Price = 99.99m,
            Description = "Test Description",
            Category = "Test Category",
            ImagePath = "test-image.jpg"
        };

        // Act
        var viewModel = _mapper.Map<ShopItemViewModel>(shopItem);

        // Assert
        Assert.NotNull(viewModel);
        Assert.Equal(shopItem.Id, viewModel.Id);
        Assert.Equal(shopItem.Name, viewModel.Name);
        Assert.Equal(shopItem.Price, viewModel.Price);
        Assert.Equal(shopItem.Description, viewModel.Description);
        Assert.Equal(shopItem.Category, viewModel.Category);
        Assert.Equal("https://images.com/test-image.jpg", viewModel.ImageUri); // Verify ImageUri is resolved correctly
    }
}