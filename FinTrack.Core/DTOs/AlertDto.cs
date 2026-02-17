
namespace FinTrack.Core.DTOs
{
    public class AlertDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; } // "Warning", "Critical", etc.
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
