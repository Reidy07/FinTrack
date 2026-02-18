using FinTrack.Core.Enum;

namespace FinTrack.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public CategoryType Type { get; set; }

        // FK a Identity (solo id; sin navegación en Core)
        public string UserId { get; set; } = default!;

        public ICollection<Income> Incomes { get; set; } = [];
        public ICollection<Expense> Expenses { get; set; } = [];
    }
}
