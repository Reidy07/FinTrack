using FinTrack.Core.DTOs;
using FinTrack.Core.DTOs.Dashboard;

namespace FinTrack.Web.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardSummaryDto Summary { get; set; } = new();

        public decimal PredictedNextMonthExpenses { get; set; }

        public decimal CurrentBalance { get; set; }

        public int TransactionsThisMonth { get; set; }

        public DateTime ReferenceDate { get; set; } = DateTime.Now;

        public bool HasData =>
            Summary.TotalIncome > 0 ||
            Summary.TotalExpenses > 0 ||
            CurrentBalance != 0 ||
            TransactionsThisMonth > 0 ||
            Summary.ChartIncomeData.Any(x => x > 0) ||
            Summary.ChartExpenseData.Any(x => x > 0) ||
            Summary.CategorySummaries.Any() ||
            Summary.RecentAlerts.Any();

        public string ReferenceMonthLabel =>
            ReferenceDate.ToString("MMMM yyyy");

        public decimal SavingsRate
        {
            get
            {
                if (Summary.TotalIncome <= 0) return 0;
                return Math.Round((Summary.Balance / Summary.TotalIncome) * 100, 2);
            }
        }
    }
}