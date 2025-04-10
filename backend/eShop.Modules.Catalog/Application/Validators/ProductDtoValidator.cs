using eShop.Modules.Catalog.Application.Dtos;
using FluentValidation;

namespace eShop.Modules.Catalog.Application.Validators;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(p => p.Category)
            .NotEmpty().WithMessage("Category is required")
            .MinimumLength(2).WithMessage("Category must be at least 2 characters")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters");

        RuleFor(p => p.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency code must be 3 characters (ISO format)")
            .Matches("^[A-Z]{3}$").WithMessage("Currency must be a valid ISO currency code (e.g., USD, EUR)");

        RuleFor(p => p.CreatedAt)
            .NotEmpty().WithMessage("Created date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Created date cannot be in the future");

        RuleFor(p => p.UpdatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Updated date cannot be in the future")
            .GreaterThanOrEqualTo(p => p.CreatedAt).When(p => p.UpdatedAt.HasValue).WithMessage("Updated date must be after created date");
    }
}