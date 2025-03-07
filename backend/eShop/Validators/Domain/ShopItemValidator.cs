using eShop.Models.Domain;
using FluentValidation;

namespace eShop.Validators.Domain;

public class ShopItemValidator : AbstractValidator<ShopItem>
{
    public ShopItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0.01m).WithMessage("Price must be greater than $0.01.")
            .LessThan(100000).WithMessage("Price must be less than $100,000.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.");

        RuleFor(x => x.ImagePath)
            .MaximumLength(200).WithMessage("Image path cannot exceed 200 characters.");
    }
}