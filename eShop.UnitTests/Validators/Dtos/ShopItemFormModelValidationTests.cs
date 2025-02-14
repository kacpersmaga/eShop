using eShop.Models.Dtos;
using eShop.Validators.Dtos;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace UnitTests.Validators.Dtos;

public class ShopItemFormModelValidatorTests
{
    private static IList<ValidationFailure> ValidateModel(ShopItemFormModel model)
    {
        var validator = new ShopItemFormModelValidator();
        var result = validator.Validate(model);
        return result.Errors;
    }

    [Fact]
    public void ShopItemFormModel_Should_Require_Name()
    {
        // Arrange
        var model = new ShopItemFormModel { Name = "", Price = 10, Category = "Test", Description = "Valid description" };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Name));
    }

    [Fact]
    public void ShopItemFormModel_Name_Should_Not_Exceed_100_Characters()
    {
        // Arrange
        var longName = new string('a', 101);
        var model = new ShopItemFormModel { Name = longName, Price = 10, Category = "Test", Description = "Valid description" };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Name));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(-1, true)]
    [InlineData(0.01, false)]
    [InlineData(99999.99, false)]
    [InlineData(100000, true)]
    public void ShopItemFormModel_Should_Validate_Price(decimal price, bool shouldHaveError)
    {
        // Arrange
        var model = new ShopItemFormModel { Name = "Test", Price = price, Category = "Test", Description = "Valid description" };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        if (shouldHaveError)
        {
            Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Price));
        }
        else
        {
            Assert.DoesNotContain(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Price));
        }
    }

    [Fact]
    public void ShopItemFormModel_Should_Require_Description()
    {
        // Arrange
        var model = new ShopItemFormModel { Name = "Test", Price = 10, Category = "Test", Description = "" };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Description));
    }

    [Fact]
    public void ShopItemFormModel_Description_Should_Not_Exceed_500_Characters()
    {
        // Arrange
        var longDescription = new string('a', 501);
        var model = new ShopItemFormModel { Name = "Test", Price = 10, Category = "Test", Description = longDescription };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Description));
    }

    [Fact]
    public void ShopItemFormModel_Should_Require_Category()
    {
        // Arrange
        var model = new ShopItemFormModel { Name = "Test", Price = 10, Category = "", Description = "Valid description" };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Category));
    }

    [Fact]
    public void ShopItemFormModel_Category_Should_Not_Exceed_50_Characters()
    {
        // Arrange
        var longCategory = new string('a', 51);
        var model = new ShopItemFormModel { Name = "Test", Price = 10, Category = longCategory, Description = "Valid description" };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Category));
    }

    [Theory]
    [InlineData("image.jpg", false)]
    [InlineData("image.png", false)]
    [InlineData("image.gif", false)]
    [InlineData("image.bmp", true)]
    [InlineData("image.txt", true)]
    public void ShopItemFormModel_Should_Validate_Image_FileType(string fileName, bool shouldHaveError)
    {
        // Arrange
        var fileMock = new FormFile(new MemoryStream(new byte[100]), 0, 100, "Data", fileName);
        var model = new ShopItemFormModel { Name = "Test", Price = 10, Category = "Test", Description = "Valid description", Image = fileMock };

        // Act
        var validationResults = ValidateModel(model);

        // Assert
        if (shouldHaveError)
        {
            Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Image));
        }
        else
        {
            Assert.DoesNotContain(validationResults, v => v.PropertyName == nameof(ShopItemFormModel.Image));
        }
    }
}

