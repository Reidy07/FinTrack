
using FinTrack.Core.Enum;

namespace FinTrack.Core.Entities
{
    public class Alert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        // Foreign Keys
        public string UserId { get; set; }
        public int? RelatedEntityId { get; set; }  // ID de gasto, presupuesto, etc.
        public string RelatedEntityType { get; set; } // "Expense", "Budget", etc.

        
        // Navigation
        public User User { get; set; }
    }
}
