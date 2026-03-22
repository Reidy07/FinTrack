using FinTrack.Core.DTOs.Alerts;
using FinTrack.Core.DTOs.Category;

namespace FinTrack.Core.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }

        public List<string> ChartLabels { get; set; } = [];
        public List<decimal> ChartIncomeData { get; set; } = [];
        public List<decimal> ChartExpenseData { get; set; } = [];
        public List<AlertDto> RecentAlerts { get; set; } = [];
        public List<CategorySummaryDto> CategorySummaries { get; set; } = [];

    }
}
