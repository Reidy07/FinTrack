using FinTrack.Core.Constants;
using FinTrack.Core.Entities;
using FluentValidation;

namespace FinTrack.Core.Validators
{
    public class ExpenseValidator : AbstractValidator<Expense>
    {
        public ExpenseValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage(ErrorMessages.AmountGreaterThanZero)
                .LessThanOrEqualTo(1000000000).WithMessage(ErrorMessages.MaxAmountExceeded);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(ErrorMessages.RequiredDescription)
                .MaximumLength(200).WithMessage(ErrorMessages.MaxLength200);

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Now).WithMessage(ErrorMessages.FutureDateNotAllowed);
        }
    }
}
