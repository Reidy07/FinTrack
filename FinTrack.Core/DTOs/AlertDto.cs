
namespace FinTrack.Core.DTOs
{
    public class AlertDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // "Warning", "Critical", etc.
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
