using FinTrack.Core.DTOs;

namespace FinTrack.Core.Interfaces.Services
{
    public interface IFinancialService
    {
        Task<ExpenseDto> AddExpenseAsync(ExpenseDto expenseDto, string userId);
        Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto, string userId);
        Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<IncomeDto>> GetIncomesByUserAsync(string userId, DateTime? startDate, DateTime? endDate);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId, DateTime? month);
        Task<decimal> GetCurrentBalanceAsync(string userId);
    }
}
