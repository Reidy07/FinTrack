using FinTrack.Core.Constants;
using FinTrack.Core.Entities;
using FluentValidation;

namespace FinTrack.Core.Validators
{
    public class IncomeValidator : AbstractValidator<Income>
    {
        public IncomeValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage(ErrorMessages.AmountGreaterThanZero);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ErrorMessages.RequiredDescription)
                .MaximumLength(200);
        }
    }
}
