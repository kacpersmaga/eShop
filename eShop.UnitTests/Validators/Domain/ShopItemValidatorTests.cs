using eShop.Models.Domain;
using eShop.Validators.Domain;
using FluentValidation.Results;

namespace UnitTests.Validators.Domain;

public class ShopItemValidatorTests
{
    private static IList<ValidationFailure> ValidateModel(ShopItem model)
    {
        var validator = new ShopItemValidator();
        var result = validator.Validate(model);
        return result.Errors;
    }

    [Fact]
    public void ShopItem_Should_Require_Name()
    {
        var model = new ShopItem { Name = "", Price = 10, Category = "Test" };
        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItem.Name));
    }

    [Fact]
    public void ShopItem_Price_Should_Be_In_Valid_Range()
    {
        var model = new ShopItem { Name = "Test", Price = 100001, Category = "Test" };
        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItem.Price));
    }

    [Fact]
    public void ShopItem_ImagePath_Should_Not_Exceed_200_Characters()
    {
        var model = new ShopItem { Name = "Test", Price = 10, Category = "Test", ImagePath = new string('a', 201) };
        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.PropertyName == nameof(ShopItem.ImagePath));
    }
}
