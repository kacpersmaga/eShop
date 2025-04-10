using eShop.Modules.Catalog.Application.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace eShop.Modules.Catalog.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
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

        RuleFor(p => p.Image)
            .Must(BeValidImage).When(p => p.Image != null).WithMessage("Invalid image format. Allowed formats are: jpg, jpeg, png");
    }

    private bool BeValidImage(IFormFile? file)
    {
        if (file == null)
            return true;
        
        if (file.Length > 5 * 1024 * 1024)
            return false;
        
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return allowedExtensions.Contains(fileExtension);
    }
}