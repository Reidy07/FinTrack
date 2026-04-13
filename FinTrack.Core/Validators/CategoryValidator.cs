using FinTrack.Core.Constants;
using FinTrack.Core.Entities;
using FluentValidation;

namespace FinTrack.Core.Validators
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ErrorMessages.RequiredName)
                .MaximumLength(100).WithMessage(ErrorMessages.MaxLength100);

            RuleFor(x => x.Color)
                .Matches("^#[0-9A-Fa-f]{6}$").WithMessage(ErrorMessages.InvalidColorFormat);
        }
    }
}
