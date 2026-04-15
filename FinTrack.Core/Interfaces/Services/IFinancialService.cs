using FinTrack.Core.DTOs.Alerts;
using FinTrack.Core.DTOs.Category;
using FinTrack.Core.DTOs.Dashboard;
using FinTrack.Core.DTOs.Expenses;
using FinTrack.Core.DTOs.Incomes;
using FinTrack.Core.DTOs.Reports;

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
        Task<IncomeDto?> GetIncomeByIdAsync(int id, string userId);
        Task UpdateIncomeAsync(IncomeDto incomeDto, string userId);
        Task DeleteIncomeAsync(int id, string userId);


        // Categorías
        Task<IEnumerable<CategoryDto>> GetCategoriesByUserAsync(string userId);
        Task<CategoryDetailDto?> GetCategoryDetailsAsync(int categoryId, string userId);
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

        // Alertas
        Task<IEnumerable<AlertDto>> GetAlertsAsync(string userId);
        Task<int> GetUnreadAlertCountAsync(string userId);
        Task MarkAlertAsReadAsync(int id, string userId);
        Task MarkAllAlertsAsReadAsync(string userId);
    }
}
