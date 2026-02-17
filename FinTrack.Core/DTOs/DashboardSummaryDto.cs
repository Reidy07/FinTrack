namespace FinTrack.Core.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }

        public List<string> ChartLabels { get; set; } = new();   // Ejem: ["Sep", "Oct"...]
        public List<decimal> ChartIncomeData { get; set; } = new(); // Ejem: [1200, 1500...]
        public List<decimal> ChartExpenseData { get; set; } = new(); // Ejem: [800, 900...]

        // Propiedad para las Alertas
        public List<AlertDto> RecentAlerts { get; set; } = new();

        public List<CategorySummaryDto> CategorySummaries { get; set; }
    }
}
