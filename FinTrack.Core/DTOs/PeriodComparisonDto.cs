namespace FinTrack.Core.DTOs
{
    public class PeriodComparisonDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Incomes { get; set; } = new();
        public List<decimal> Expenses { get; set; } = new();
    }
}
