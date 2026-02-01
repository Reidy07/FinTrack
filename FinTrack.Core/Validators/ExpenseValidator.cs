using FinTrack.Core.Entities;
using FluentValidation;

namespace FinTrack.Core.Validators
{
    public class ExpenseValidator: AbstractValidator<Expense>
    {
        public ExpenseValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.")
                .LessThanOrEqualTo(1000000000).WithMessage("El monto no puede exceder $1,000,000,000");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres.");

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser futura.");
        }
    }
}
