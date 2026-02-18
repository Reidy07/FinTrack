
namespace FinTrack.Core.DTOs
{
    public class CategorySummaryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
