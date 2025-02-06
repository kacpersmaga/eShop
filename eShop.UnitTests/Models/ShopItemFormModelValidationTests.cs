using System.ComponentModel.DataAnnotations;
using eShop.Models.Domain;

namespace UnitTests.Models;

public class ShopItemFormModelValidationTests
{
    [Fact]
    public void ShopItem_Should_Have_DefaultValues()
    {
        // Arrange
        var shopItem = new ShopItem { Name = "", Category = "", Price = 0 };

        // Assert
        Assert.Equal(string.Empty, shopItem.Name);
        Assert.Equal(string.Empty, shopItem.Category);
        Assert.Null(shopItem.Description);
        Assert.Null(shopItem.ImagePath);
        Assert.Equal(0, shopItem.Price);
    }

    [Fact]
    public void ShopItem_Should_Require_Name()
    {
        // Arrange
        var shopItem = new ShopItem { Name = "", Category = "Test", Price = 10 };

        // Act
        var validationResults = ValidateModel(shopItem);

        // Assert
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(ShopItem.Name)));
    }

    [Fact]
    public void ShopItem_Should_Require_Category()
    {
        // Arrange
        var shopItem = new ShopItem { Name = "Test", Category = "", Price = 10 };

        // Act
        var validationResults = ValidateModel(shopItem);

        // Assert
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(ShopItem.Category)));
    }

    [Theory]
    [InlineData(-1, true)] 
    [InlineData(0, true)]
    [InlineData(0.01, false)]
    [InlineData(100000, false)]
    [InlineData(100001, true)]
    public void ShopItem_Should_Validate_Price(decimal price, bool shouldHaveError)
    {
        // Arrange
        var shopItem = new ShopItem { Name = "Test", Category = "Test", Price = price };

        // Act
        var validationResults = ValidateModel(shopItem);

        // Assert
        if (shouldHaveError)
        {
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(ShopItem.Price)));
        }
        else
        {
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(ShopItem.Price)));
        }
    }

    [Fact]
    public void ShopItem_Should_Validate_Description_Length()
    {
        // Arrange
        var validDescription = new string('a', 500);
        var invalidDescription = new string('a', 501);

        var shopItemValid = new ShopItem { Name = "Test", Category = "Test", Price = 10, Description = validDescription };
        var shopItemInvalid = new ShopItem { Name = "Test", Category = "Test", Price = 10, Description = invalidDescription };

        // Act
        var validationResultsValid = ValidateModel(shopItemValid);
        var validationResultsInvalid = ValidateModel(shopItemInvalid);

        // Assert
        Assert.Empty(validationResultsValid);
        Assert.Contains(validationResultsInvalid, v => v.MemberNames.Contains(nameof(ShopItem.Description)));
    }

    [Fact]
    public void ShopItem_Should_Validate_Required_Fields()
    {
        // Arrange
        var shopItem = new ShopItem { Name = "", Category = "", Price = 10 };

        // Act
        var validationResults = ValidateModel(shopItem);

        // Assert
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(ShopItem.Name)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(ShopItem.Category)));
    }

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, validationResults, true);
        return validationResults;
    }
}
