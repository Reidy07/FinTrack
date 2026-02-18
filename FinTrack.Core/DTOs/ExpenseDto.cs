
namespace FinTrack.Core.DTOs
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string UserId { get; set; } = null!;
    }
}
