using FinTrack.Core.Interfaces;

namespace FinTrack.Core.Entities
{
    // Patrón de diseño: Adaptador
public class User : IUser
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}

}
