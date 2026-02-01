using FinTrack.Core.Interfaces;

namespace FinTrack.Core.Entities
{
    // Patrón de diseño: Adaptador
    public class User: IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Income> Incomes { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Prediction> Predictions { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
    }
}
