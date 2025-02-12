using eShop.Models.Dtos;
using FluentValidation;

namespace eShop.Validators.Dtos;

public class ShopItemFormModelValidator : AbstractValidator<ShopItemFormModel>
{
    public ShopItemFormModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThan(100000).WithMessage("Price must be less than 100,000.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.");

        RuleFor(x => x.Image)
            .Must(BeAValidImage).WithMessage("Only .jpg, .png, and .gif files are allowed.");
    }

    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null) return true;
        var allowedExtensions = new[] { ".jpg", ".png", ".gif" };
        return allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
    }
}