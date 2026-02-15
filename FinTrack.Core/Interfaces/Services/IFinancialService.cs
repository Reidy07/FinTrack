using FinTrack.Core.DTOs;

namespace FinTrack.Core.Interfaces.Services
{
    public interface IFinancialService
    {
        // Gastos
        Task<ExpenseDto> AddExpenseAsync(ExpenseDto expenseDto, string userId);
        Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId, DateTime? startDate, DateTime? endDate);

        // Ingresos
        Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto, string userId);
        Task<IEnumerable<IncomeDto>> GetIncomesByUserAsync(string userId, DateTime? startDate, DateTime? endDate);

        // Dashboard
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId, DateTime? month);
        Task<decimal> GetCurrentBalanceAsync(string userId);
    }
}
