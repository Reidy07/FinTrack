namespace FinTrack.Core.DTOs.Category
{
    public class CategorySummaryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
