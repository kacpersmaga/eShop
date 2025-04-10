using eShop.Modules.Catalog.Application.Dtos;
using FluentValidation;

namespace eShop.Modules.Catalog.Application.Validators;

public class PagedProductsDtoValidator : AbstractValidator<PagedProductsDto>
{
    public PagedProductsDtoValidator()
    {
        RuleFor(p => p.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero");

        RuleFor(p => p.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100 items per page");

        RuleFor(p => p.TotalItems)
            .GreaterThanOrEqualTo(0).WithMessage("Total items cannot be negative");

        RuleFor(p => p.TotalPages)
            .GreaterThanOrEqualTo(0).WithMessage("Total pages cannot be negative");

        RuleFor(p => p.Items)
            .NotNull().WithMessage("Items collection cannot be null");
            
        RuleForEach(p => p.Items)
            .SetValidator(new ProductDtoValidator());
    }
}