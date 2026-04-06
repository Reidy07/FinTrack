using FinTrack.Core.DTOs.Alerts;
using FinTrack.Core.DTOs.Category;
using FinTrack.Core.DTOs.Dashboard;
using FinTrack.Core.DTOs.Expenses;
using FinTrack.Core.DTOs.Incomes;
using FinTrack.Core.DTOs.Reports;
using FinTrack.Core.Entities;
using FinTrack.Core.Enum;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;


namespace FinTrack.Core.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IEmailTemplateService _templateService;

        public FinancialService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IUserService userService,
            IEmailTemplateService templateService)
        {
            this.unitOfWork = unitOfWork;
            _emailService = emailService;
            _userService = userService;
            _templateService = templateService;
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
                UserId = userId,
                IsRecurring = dto.IsRecurring,
                RecurringPattern = dto.IsRecurring ? dto.RecurringPattern : null
            };

            await unitOfWork.Expenses.AddAsync(expense);
            await unitOfWork.CompleteAsync();

            // 1. Verificar balance negativo.
            var currentBalance = await GetCurrentBalanceAsync(userId);
            if (currentBalance < 0)
            {
                var alertTitle = "¡Cuidado! Balance en rojo";
                var alertMsg = $"Tu último gasto de {dto.Amount:C} ha dejado tu cuenta en negativo ({currentBalance:C}). Revisa tus finanzas.";

                await unitOfWork.Alerts.AddAsync(new Alert
                {
                    UserId = userId,
                    Title = alertTitle,
                    Message = alertMsg,
                    Type = AlertType.UnusualSpending,
                    Severity = AlertSeverity.Critical,
                    CreatedAt = DateTime.UtcNow
                });
                await unitOfWork.CompleteAsync();

                // Enviar el correo de Balance Negativo con la plantilla HTML
                var userEmail = await _userService.GetUserEmailAsync(userId);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var alertHtml = await _templateService.GetAlertTemplateAsync(
                        title: alertTitle,
                        message: alertMsg,
                        actionUrl: "https://localhost:7127/Dashboard",
                        actionText: "Ir al Dashboard");

                    await _emailService.SendEmailAsync(userEmail, alertTitle, alertHtml);
                }
            }

            dto.Id = expense.Id;

            // Devolver nombre de categoría si existe
            var cat = await unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            dto.CategoryName = cat?.Name ?? dto.CategoryName ?? "General";

            // 2. Revisar si el gasto excede el presupuesto.
            var budgets = await unitOfWork.Budgets.FindAsync(b => b.CategoryId == dto.CategoryId && b.UserId == userId);
            var budget = budgets.FirstOrDefault(b => b.StartDate <= dto.Date && b.EndDate >= dto.Date);

            if (budget != null)
            {
                var currentExpenses = await unitOfWork.Expenses.FindAsync(e =>
                    e.CategoryId == dto.CategoryId && e.UserId == userId && e.Date >= budget.StartDate && e.Date <= budget.EndDate);

                var totalSpent = currentExpenses.Sum(e => e.Amount);

                if (totalSpent > budget.Amount)
                {
                    var budgetTitle = "¡Presupuesto Excedido!";
                    var budgetMsg = $"Has superado tu presupuesto de {budget.Amount:C} para la categoría '{budget.Category?.Name ?? "General"}'. Llevas gastado {totalSpent:C}.";

                    await unitOfWork.Alerts.AddAsync(new Alert
                    {
                        UserId = userId,
                        Title = budgetTitle,
                        Message = budgetMsg,
                        Type = AlertType.BudgetExceeded,
                        Severity = AlertSeverity.Warning,
                        CreatedAt = DateTime.UtcNow
                    });
                    await unitOfWork.CompleteAsync();

                    // Enviar también un correo si se excede el presupuesto
                    var userEmail = await _userService.GetUserEmailAsync(userId);
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var alertHtml = await _templateService.GetAlertTemplateAsync(
                            title: budgetTitle,
                            message: budgetMsg,
                            actionUrl: "https://localhost:7127/Dashboard",
                            actionText: "Ver mis presupuestos");

                        await _emailService.SendEmailAsync(userEmail, budgetTitle, alertHtml);
                    }
                }
            }

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
                    UserId = e.UserId,
                    IsRecurring = e.IsRecurring,
                    RecurringPattern = e.RecurringPattern
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
                CategoryName = cat?.Name ?? "General",
                IsRecurring = expense.IsRecurring,
                RecurringPattern = expense.RecurringPattern
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
            expense.IsRecurring = dto.IsRecurring;
            expense.RecurringPattern = dto.IsRecurring ? dto.RecurringPattern : null;

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

        public async Task<IncomeDto?> GetIncomeByIdAsync(int id, string userId)
        {
            var income = await unitOfWork.Incomes.GetByIdAsync(id);
            if (income == null || income.UserId != userId) return null;

            return new IncomeDto
            {
                Id = income.Id,
                Amount = income.Amount,
                Description = income.Description,
                Date = income.Date,
                CategoryId = income.CategoryId
            };
        }

        public async Task UpdateIncomeAsync(IncomeDto dto, string userId)
        {
            var income = await unitOfWork.Incomes.GetByIdAsync(dto.Id);
            if (income == null || income.UserId != userId) return;

            income.Amount = dto.Amount;
            income.Description = dto.Description;
            income.Date = dto.Date;
            income.CategoryId = dto.CategoryId;

            unitOfWork.Incomes.Update(income);
            await unitOfWork.CompleteAsync();
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
        public async Task<CategoryDetailDto?> GetCategoryDetailsAsync(int categoryId, string userId)
        {
            // 1. Buscamos la categoría
            var category = await unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null || category.UserId != userId) return null;

            // 2. Traemos todos sus gastos e ingresos asociados
            var expenses = await unitOfWork.Expenses.FindAsync(e => e.CategoryId == categoryId && e.UserId == userId);
            var incomes = await unitOfWork.Incomes.FindAsync(i => i.CategoryId == categoryId && i.UserId == userId);

            // 3. Unificamos todo en una sola lista de "Transacciones"
            var transactions = new List<CategoryTransactionDto>();
            transactions.AddRange(expenses.Select(e => new CategoryTransactionDto { Id = e.Id, Amount = e.Amount, Description = e.Description, Date = e.Date, TransactionType = "Gasto" }));
            transactions.AddRange(incomes.Select(i => new CategoryTransactionDto { Id = i.Id, Amount = i.Amount, Description = i.Description, Date = i.Date, TransactionType = "Ingreso" }));

            var orderedTransactions = transactions.OrderByDescending(t => t.Date).ToList();
            var totalAmount = orderedTransactions.Sum(t => t.Amount);

            // 4. Calculamos Mes Actual y Mes Anterior
            var now = DateTime.Now;
            var currentMonthTotal = orderedTransactions.Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month).Sum(t => t.Amount);

            var prevMonth = now.AddMonths(-1);
            var previousMonthTotal = orderedTransactions.Where(t => t.Date.Year == prevMonth.Year && t.Date.Month == prevMonth.Month).Sum(t => t.Amount);

            // 5. Agrupamos por mes para gráficas o listas (Ej: "Ene 2026", "Feb 2026")
            var culture = new System.Globalization.CultureInfo("es-ES");
            var monthlyTotals = orderedTransactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", culture).ToUpper(),
                    g => g.Sum(t => t.Amount)
                );

            return new CategoryDetailDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Color = category.Color ?? "#3498db",
                Type = category.Type.ToString(),
                TotalAmount = totalAmount,
                CurrentMonthAmount = currentMonthTotal,
                PreviousMonthAmount = previousMonthTotal,
                MonthlyTotals = monthlyTotals,
                Transactions = orderedTransactions
            };
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

            // 1. Totales del Mes actual (Entradas y salidas de este mes)
            var expenses = await unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= startOfMonth && e.Date <= endOfMonth);
            var incomes = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= startOfMonth && i.Date <= endOfMonth);

            var totalExp = expenses.Sum(e => e.Amount);
            var totalInc = incomes.Sum(i => i.Amount);

            // 2. Balance histórico acumulado
            var accumulatedBalance = await GetCurrentBalanceAsync(userId);

            // 3. Datos para el Gráfico (Últimos 6 meses)
            var chartLabels = new List<string>();
            var chartIncome = new List<decimal>();
            var chartExpense = new List<decimal>();

            for (int i = 5; i >= 0; i--)
            {
                var date = referenceDate.AddMonths(-i);
                var monthStart = new DateTime(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthlyExp = await unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= monthStart && e.Date <= monthEnd);
                var monthlyInc = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= monthStart && i.Date <= monthEnd);

                chartLabels.Add(date.ToString("MMM"));
                chartIncome.Add(monthlyInc.Sum(x => x.Amount));
                chartExpense.Add(monthlyExp.Sum(x => x.Amount));
            }

            // 4. Alertas recientes (Top 3)
            var alerts = await unitOfWork.Alerts.FindAsync(a => a.UserId == userId);
            var recentAlerts = alerts
                .OrderByDescending(a => a.CreatedAt)
                .Take(3)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    Severity = a.Severity.ToString(),
                    CreatedAt = a.CreatedAt,
                    IsRead = a.IsRead
                })
                .ToList();

            // 5. Resumen por Categorías
            var categories = await unitOfWork.Categories.GetAllAsync();
            var catMap = categories.ToDictionary(c => c.Id, c => c.Name);

            var categorySummaries = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new CategorySummaryDto
                {
                    CategoryName = catMap.TryGetValue(g.Key, out var name) ? name : "General",
                    Amount = g.Sum(e => e.Amount),

                    // Calculamos el porcentaje respecto al total gastado en el mes
                    Percentage = totalExp > 0 ? Math.Round((g.Sum(e => e.Amount) / totalExp) * 100, 2) : 0
                })
                .OrderByDescending(c => c.Amount) // Ordenamos de la categoría con más gasto a la de menos
                .ToList();

            // 6. Retornamos mapeando exactamente todo el DTO
            return new DashboardSummaryDto
            {
                TotalExpenses = totalExp,
                TotalIncome = totalInc,
                Balance = accumulatedBalance,
                ChartLabels = chartLabels,
                ChartIncomeData = chartIncome,
                ChartExpenseData = chartExpense,
                RecentAlerts = recentAlerts,
                CategorySummaries = categorySummaries
            };
        }

        public async Task<decimal> GetCurrentBalanceAsync(string userId)
        {
            var now = DateTime.Now;
            // Solo tomamos en cuenta los ingresos y gastos hasta hoy
            var incomes = await unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date <= now);
            var expenses = await unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date <= now);

            return incomes.Sum(i => i.Amount) - expenses.Sum(e => e.Amount);
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

        public async Task MarkAlertAsReadAsync(int alertId, string userId)
        {
            var alert = await unitOfWork.Alerts.GetByIdAsync(alertId);
            if (alert != null && alert.UserId == userId)
            {
                alert.IsRead = true;
                unitOfWork.Alerts.Update(alert);
                await unitOfWork.CompleteAsync();
            }
        }
    }
}
