using FinTrack.Core.Enum;

namespace FinTrack.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }

        public CategoryType Type { get; set; } 

        // Foreign Key
        public string UserId { get; set; }

        // Navigation
        public User User { get; set; }
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
