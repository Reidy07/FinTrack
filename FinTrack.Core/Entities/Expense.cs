namespace FinTrack.Core.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        
        // Para análisis ML
        public bool IsRecurring { get; set; } = false;
        public string RecurringPattern { get; set; } // "Monthly", "Weekly", null

        
        // Foreign Keys
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        
        // Navigation
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
