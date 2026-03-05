using FinTrack.Core.DTOs;

namespace FinTrack.Core.Interfaces.Services
{
    public interface IReportService
    {
        Task<PeriodComparisonDto> GetPeriodComparisonAsync(string userId, int months = 6);
        Task<BalanceTrendDto> GetBalanceTrendAsync(string userId, int months = 6);
        Task<CategoryComparisonDto> GetCategoryComparisonAsync(string userId, int months = 6);
    }
}
