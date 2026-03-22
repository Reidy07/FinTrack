using FinTrack.Core.DTOs.Category;

namespace FinTrack.Core.DTOs
{
    public class GeminiPredictionRequestDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
        public List<CategorySummaryDto> TopCategories { get; set; } = new();
        public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new();
        public List<string> UserPriorities { get; set; } = new();
        public decimal? SavingsGoalPercentage { get; set; }
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
    }

    public class GeminiPredictionResultDto
    {
        public string Summary { get; set; } = string.Empty;
        public decimal PredictedNextMonthExpenses { get; set; }
        public decimal RecommendedSavings { get; set; }
        public decimal FreeToSpend { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public List<PaymentReminderDto> PaymentReminders { get; set; } = new();
        public List<CategoryAdviceDto> CategoryAdvice { get; set; } = new();
        public string HealthStatus { get; set; } = "Good";
    }

    public class PaymentReminderDto
    {
        public string Name { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public decimal EstimatedAmount { get; set; }
    }

    public class CategoryAdviceDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal CurrentSpending { get; set; }
        public decimal RecommendedMax { get; set; }
        public string Advice { get; set; } = string.Empty;
    }
}