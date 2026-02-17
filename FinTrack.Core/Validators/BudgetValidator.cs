using FinTrack.Core.Entities;
using FluentValidation;

namespace FinTrack.Core.Validators
{
    public class BudgetValidator : AbstractValidator<Budget>
    {
        public BudgetValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty().GreaterThan(x => x.StartDate)
                .WithMessage("La fecha de fin debe ser posterior a la de inicio.");
        }
    }
}
