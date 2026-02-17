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

        public async Task DeleteExpenseAsync(int id, string userId)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
            if (expense != null && expense.UserId == userId)
            {
                _unitOfWork.Expenses.Delete(expense);
                await _unitOfWork.CompleteAsync();
            }
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

        public async Task DeleteIncomeAsync(int id, string userId)
        {
            var income = await _unitOfWork.Incomes.GetByIdAsync(id);
            if (income != null && income.UserId == userId)
            {
                _unitOfWork.Incomes.Delete(income);
                await _unitOfWork.CompleteAsync();
            }
        }

         // --- CATEGORÍAS ---
        public async Task<IEnumerable<CategoryDto>> GetCategoriesByUserAsync(string userId)
        {
            var categories = await _unitOfWork.Categories.FindAsync(c => c.UserId == userId);
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                Description = c.Description,
                Type = c.Type
            });
        }

        public async Task<CategoryDto> AddCategoryAsync(CategoryDto dto, string userId)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color ?? "#3498db",
                Type = dto.Type,
                UserId = userId
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            dto.Id = category.Id;
            return dto;
        }

        // --- PRESUPUESTOS ---
        public async Task<IEnumerable<BudgetDto>> GetBudgetsByUserAsync(string userId)
        {
            var budgets = await _unitOfWork.Budgets.FindAsync(b => b.UserId == userId);
            var categories = await _unitOfWork.Categories.GetAllAsync(); // Cargar caché o mejorar query

            return budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,
                Amount = b.Amount,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                CategoryId = b.CategoryId,
                CategoryName = b.CategoryId.HasValue
                    ? categories.FirstOrDefault(c => c.Id == b.CategoryId)?.Name
                    : "Global"
            });
        }

        public async Task<BudgetDto> AddBudgetAsync(BudgetDto dto, string userId)
        {
            var budget = new Budget
            {
                Name = dto.Name,
                Amount = dto.Amount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CategoryId = dto.CategoryId,
                UserId = userId
            };

            await _unitOfWork.Budgets.AddAsync(budget);
            await _unitOfWork.CompleteAsync();

            dto.Id = budget.Id;
            return dto;
        }


        // --- DASHBOARD ---
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId, DateTime? dateRef)
        {
            var referenceDate = dateRef ?? DateTime.Now;
            var startOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // 1. Totales del Mes actual
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= startOfMonth && e.Date <= endOfMonth);
            var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= startOfMonth && i.Date <= endOfMonth);

            var totalExp = expenses.Sum(e => e.Amount);
            var totalInc = incomes.Sum(i => i.Amount);

            // 2. Datos para el Gráfico (Últimos 6 meses)
            var chartLabels = new List<string>();
            var chartIncome = new List<decimal>();
            var chartExpense = new List<decimal>();

            // Iteramos 5 meses atrás hasta hoy
            for (int i = 5; i >= 0; i--)
            {
                var date = referenceDate.AddMonths(-i);
                var monthStart = new DateTime(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // Consultamos BD para ese mes específico (Nota: esto se puede optimizar luego con 1 sola query, pero para empezar así es más claro)
                var monthlyExp = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= monthStart && e.Date <= monthEnd);
                var monthlyInc = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= monthStart && i.Date <= monthEnd);

                // Formato etiqueta: "Ene", "Feb", etc.
                chartLabels.Add(date.ToString("MMM"));
                chartIncome.Add(monthlyInc.Sum(x => x.Amount));
                chartExpense.Add(monthlyExp.Sum(x => x.Amount));
            }

            // 3. Alertas recientes (Top 3)
            var alerts = await _unitOfWork.Alerts.FindAsync(a => a.UserId == userId);
            var recentAlerts = alerts
                .OrderByDescending(a => a.CreatedAt)
                .Take(3)
                .Select(a => new AlertDto
                {
                    Title = a.Title,
                    Message = a.Message,
                    Severity = a.Severity.ToString(),
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            return new DashboardSummaryDto
            {
                TotalExpenses = totalExp,
                TotalIncome = totalInc,
                Balance = totalInc - totalExp,
                ChartLabels = chartLabels,
                ChartIncomeData = chartIncome,
                ChartExpenseData = chartExpense,
                RecentAlerts = recentAlerts
            };
        }

        public async Task<decimal> GetCurrentBalanceAsync(string userId)
        {
            // Suma de ingresos
            var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId);
            var totalIncome = incomes.Sum(i => i.Amount);

            // Suma de gastos
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId);
            var totalExpense = expenses.Sum(e => e.Amount);

            return totalIncome - totalExpense;
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsAsync(string userId)
        {
            var alerts = await _unitOfWork.Alerts.FindAsync(a => a.UserId == userId);
            return alerts.OrderByDescending(a => a.CreatedAt).Select(a => new AlertDto
            {
                Id = a.Id,
                Title = a.Title,
                Message = a.Message,
                Severity = a.Severity.ToString(),
                CreatedAt = a.CreatedAt,
                IsRead = a.IsRead
            });
        }
    }
}