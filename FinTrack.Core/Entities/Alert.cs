
using FinTrack.Core.Enum;

namespace FinTrack.Core.Entities
{
    public class Alert
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } = string.Empty;
    }
}
