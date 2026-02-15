using FinTrack.Core.DTOs;
using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;

namespace FinTrack.Core.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinancialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- GASTOS ---
        public async Task<ExpenseDto> AddExpenseAsync(ExpenseDto dto, string userId)
        {
            var expense = new Expense
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                UserId = userId
            };
            await _unitOfWork.Expenses.AddAsync(expense);
            await _unitOfWork.CompleteAsync();
            return dto;
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId);
            // Cargar categorías (simulado, idealmente usar Include en repositorio)
            var categories = await _unitOfWork.Categories.GetAllAsync();

            return expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                Date = e.Date,
                CategoryId = e.CategoryId,
                CategoryName = categories.FirstOrDefault(c => c.Id == e.CategoryId)?.Name ?? "General"
            });
        }

        // --- INGRESOS ---
        public async Task<IncomeDto> AddIncomeAsync(IncomeDto dto, string userId)
        {
            var income = new Income
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                UserId = userId
            };
            await _unitOfWork.Incomes.AddAsync(income);
            await _unitOfWork.CompleteAsync();
            return dto;
        }

        public async Task<IEnumerable<IncomeDto>> GetIncomesByUserAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId);
            return incomes.Select(i => new IncomeDto
            {
                Id = i.Id,
                Amount = i.Amount,
                Description = i.Description,
                Date = i.Date
            });
        }

        // --- DASHBOARD ---
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId, DateTime? month)
        {
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId);
            var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId);

            var totalExp = expenses.Sum(e => e.Amount);
            var totalInc = incomes.Sum(i => i.Amount);

            // Agrupación simple para categorías
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var catSummary = expenses.GroupBy(e => e.CategoryId)
                .Select(g => new CategorySummaryDto
                {
                    CategoryName = categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "General",
                    Amount = g.Sum(e => e.Amount),
                    Percentage = totalExp == 0 ? 0 : (g.Sum(e => e.Amount) / totalExp) * 100
                }).ToList();

            return new DashboardSummaryDto
            {
                TotalExpenses = totalExp,
                TotalIncome = totalInc,
                Balance = totalInc - totalExp,
                CategorySummaries = catSummary
            };
        }

        public Task<decimal> GetCurrentBalanceAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}