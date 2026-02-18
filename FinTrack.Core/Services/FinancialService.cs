using FinTrack.Core.DTOs;
using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;


namespace FinTrack.Core.Services
{
    public class FinancialService(IUnitOfWork unitOfWork) : IFinancialService
    {

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

            await unitOfWork.Expenses.AddAsync(expense);
            await unitOfWork.CompleteAsync();

            dto.Id = expense.Id;

            // opcional: devolver nombre de categoría si existe
            var cat = await unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            dto.CategoryName = cat?.Name ?? dto.CategoryName ?? "General";

            return dto;
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var expenses = await unitOfWork.Expenses.FindAsync(e =>
                e.UserId == userId
                && (!startDate.HasValue || e.Date >= startDate.Value)
                && (!endDate.HasValue || e.Date <= endDate.Value)
            );

            // Traemos categorías para mapear CategoryName sin N+1
            var categories = await unitOfWork.Categories.GetAllAsync();
            var catMap = categories.ToDictionary(c => c.Id, c => c.Name);

            return expenses
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date,
                    CategoryId = e.CategoryId,
                    CategoryName = catMap.TryGetValue(e.CategoryId, out var name) ? name : "General",
                    UserId = e.UserId
                });
        }
        public async Task<ExpenseDto?> GetExpenseByIdAsync(int id, string userId)
        {
            var expense = await unitOfWork.Expenses.GetByIdAsync(id);
            if (expense == null || expense.UserId != userId) return null;

            var cat = (await unitOfWork.Categories.GetAllAsync())
            .FirstOrDefault(c => c.Id == expense.CategoryId);

            return new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                CategoryName = cat?.Name ?? "General"
            };
        }

        public async Task UpdateExpenseAsync(ExpenseDto dto, string userId)
        {
            var expense = await unitOfWork.Expenses.GetByIdAsync(dto.Id);
            if (expense == null || expense.UserId != userId) return;

            expense.Amount = dto.Amount;
            expense.Description = dto.Description;
            expense.Date = dto.Date;
            expense.CategoryId = dto.CategoryId;

                unitOfWork.Expenses.Update(expense);
                await unitOfWork.CompleteAsync();
        }


        public async Task DeleteExpenseAsync(int id, string userId)
        {
            var expense = await unitOfWork.Expenses.GetByIdAsync(id);
            if (expense != null && expense.UserId == userId)
            {
                unitOfWork.Expenses.Delete(expense);
                await unitOfWork.CompleteAsync();
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
            await unitOfWork.Incomes.AddAsync(income);
            await unitOfWork.CompleteAsync();
            return dto;
        }

        public async Task<IEnumerable<IncomeDto>> GetIncomesByUserAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var incomes = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId);
            var categories = await unitOfWork.Categories.FindAsync(c => c.UserId == userId);

            var catMap = categories.ToDictionary(c => c.Id, c => c.Name);

            return incomes.Select(i => new IncomeDto
            {
                Id = i.Id,
                Amount = i.Amount,
                Description = i.Description,
                Date = i.Date,
                CategoryId = i.CategoryId,
                CategoryName = catMap.TryGetValue(i.CategoryId, out var name) ? name : "Sin categoría"
            });
        }


        public async Task DeleteIncomeAsync(int id, string userId)
        {
            var income = await unitOfWork.Incomes.GetByIdAsync(id);
            if (income != null && income.UserId == userId)
            {
                unitOfWork.Incomes.Delete(income);
                await unitOfWork.CompleteAsync();
            }
        }

         // --- CATEGORÍAS ---
        public async Task<IEnumerable<CategoryDto>> GetCategoriesByUserAsync(string userId)
        {
            var categories = await unitOfWork.Categories.FindAsync(c => c.UserId == userId);
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

            await unitOfWork.Categories.AddAsync(category);
            await unitOfWork.CompleteAsync();

            dto.Id = category.Id;
            return dto;
        }
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id, string userId)
{
    var cat = await unitOfWork.Categories.GetByIdAsync(id);
    if (cat == null || cat.UserId != userId) return null;

    return new CategoryDto
    {
        Id = cat.Id,
        Name = cat.Name,
        Description = cat.Description,
        Color = cat.Color,
        Type = cat.Type
    };
}

public async Task UpdateCategoryAsync(CategoryDto dto, string userId)
{
    var cat = await unitOfWork.Categories.GetByIdAsync(dto.Id);
    if (cat == null || cat.UserId != userId) return;

    cat.Name = dto.Name;
    cat.Description = dto.Description;
    cat.Color = string.IsNullOrWhiteSpace(dto.Color) ? "#3498db" : dto.Color;
    cat.Type = dto.Type;

    unitOfWork.Categories.Update(cat);
    await unitOfWork.CompleteAsync();
}

public async Task DeleteCategoryAsync(int id, string userId)
{
    var cat = await unitOfWork.Categories.GetByIdAsync(id);
    if (cat == null || cat.UserId != userId) return;

    unitOfWork.Categories.Delete(cat);
    await unitOfWork.CompleteAsync();
}

        // --- PRESUPUESTOS ---
        public async Task<IEnumerable<BudgetDto>> GetBudgetsByUserAsync(string userId)
        {
            var budgets = await unitOfWork.Budgets.FindAsync(b => b.UserId == userId);
            var categories = await unitOfWork.Categories.GetAllAsync(); // Cargar caché o mejorar query

            return budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,
                Amount = b.Amount,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                CategoryId = b.CategoryId,
                CategoryName = b.CategoryId.HasValue
    ? (categories.FirstOrDefault(c => c.Id == b.CategoryId)?.Name ?? "Sin categoría")
    : "Global",

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

            await unitOfWork.Budgets.AddAsync(budget);
            await unitOfWork.CompleteAsync();

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
            var expenses = await unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= startOfMonth && e.Date <= endOfMonth);
            var incomes = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= startOfMonth && i.Date <= endOfMonth);

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
                var monthlyExp = await unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= monthStart && e.Date <= monthEnd);
                var monthlyInc = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= monthStart && i.Date <= monthEnd);

                // Formato etiqueta: "Ene", "Feb", etc.
                chartLabels.Add(date.ToString("MMM"));
                chartIncome.Add(monthlyInc.Sum(x => x.Amount));
                chartExpense.Add(monthlyExp.Sum(x => x.Amount));
            }

            // 3. Alertas recientes (Top 3)
            var alerts = await unitOfWork.Alerts.FindAsync(a => a.UserId == userId);
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
            var incomes = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId);
            var totalIncome = incomes.Sum(i => i.Amount);

            // Suma de gastos
            var expenses = await unitOfWork.Expenses.FindAsync(e => e.UserId == userId);
            var totalExpense = expenses.Sum(e => e.Amount);

            return totalIncome - totalExpense;
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsAsync(string userId)
        {
            var alerts = await unitOfWork.Alerts.FindAsync(a => a.UserId == userId);
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