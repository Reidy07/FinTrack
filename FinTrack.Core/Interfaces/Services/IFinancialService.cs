using FinTrack.Core.DTOs;

namespace FinTrack.Core.Interfaces.Services
{
    public interface IFinancialService
    {
        // Gastos
        Task<ExpenseDto> AddExpenseAsync(ExpenseDto expenseDto, string userId);
        Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId, DateTime? startDate, DateTime? endDate);

        Task<ExpenseDto?> GetExpenseByIdAsync(int id, string userId);
        Task UpdateExpenseAsync(ExpenseDto expenseDto, string userId);
        Task DeleteExpenseAsync(int id, string userId);

        // Ingresos
        Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto, string userId);
        Task<IEnumerable<IncomeDto>> GetIncomesByUserAsync(string userId, DateTime? startDate, DateTime? endDate);

        // Categorías
        Task<IEnumerable<CategoryDto>> GetCategoriesByUserAsync(string userId);
        Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto, string userId);
        Task<CategoryDto?> GetCategoryByIdAsync(int id, string userId);
        Task UpdateCategoryAsync(CategoryDto dto, string userId);
        Task DeleteCategoryAsync(int id, string userId);

        // Presupuestos
        Task<IEnumerable<BudgetDto>> GetBudgetsByUserAsync(string userId);
        Task<BudgetDto> AddBudgetAsync(BudgetDto budgetDto, string userId);

        // Dashboard
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId, DateTime? month);
        Task<decimal> GetCurrentBalanceAsync(string userId);

        // Alerts
        Task<IEnumerable<AlertDto>> GetAlertsAsync(string userId);
    }
}
