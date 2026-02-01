using FinTrack.Core.DTOs;

namespace FinTrack.Core.Interfaces.Services
{
    public interface IPredictionService
    {
        Task<decimal> PredictNextMonthExpensesAsync(string userId);
        Task<Dictionary<string, decimal>> PredictCategoryTrendsAsync(string userId);
        Task<List<AlertDto>> GenerateAlertsAsync(string userId);
    }
}