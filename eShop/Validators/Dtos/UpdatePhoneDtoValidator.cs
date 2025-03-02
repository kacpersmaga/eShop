using FluentValidation;
using eShop.Models.Dtos;

namespace eShop.Validators.Dtos;

public class UpdatePhoneDtoValidator : AbstractValidator<UpdatePhoneDto>
{
    public UpdatePhoneDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be in a valid format. Use international format (e.g., +11234567890).")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
