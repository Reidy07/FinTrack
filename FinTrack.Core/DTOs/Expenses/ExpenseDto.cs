namespace FinTrack.Core.DTOs.Expenses
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string? UserId { get; set; }

        public bool IsRecurring { get; set; }
        public string? RecurringPattern { get; set; } // Ej: "Diario", "Semanal", "Mensual", "Anual"
    }
}
