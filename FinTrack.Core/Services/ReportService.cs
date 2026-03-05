using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;

namespace FinTrack.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PeriodComparisonDto> GetPeriodComparisonAsync(string userId, int months = 6)
        {
            var dto = new PeriodComparisonDto();

            if (months == 1)
            {
                // LÓGICA SEMANAL (Últimas 4 semanas)
                var startDate = DateTime.Now.Date.AddDays(-28);
                var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= startDate);
                var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= startDate);

                for (int i = 3; i >= 0; i--)
                {
                    var weekStart = DateTime.Now.Date.AddDays(-(i * 7) - 7);
                    var weekEnd = DateTime.Now.Date.AddDays(-(i * 7));

                    dto.Labels.Add($"{weekStart:dd MMM} al {weekEnd:dd MMM}"); // Ej: "01 Mar al 07 Mar"
                    dto.Expenses.Add(expenses.Where(e => e.Date > weekStart && e.Date <= weekEnd).Sum(e => e.Amount));
                    dto.Incomes.Add(incomes.Where(i => i.Date > weekStart && i.Date <= weekEnd).Sum(i => i.Amount));
                }
            }
            else
            {
                // LÓGICA MENSUAL
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-months + 1);
                var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= startDate);
                var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= startDate);

                for (int i = months - 1; i >= 0; i--)
                {
                    var monthDate = DateTime.Now.AddMonths(-i);
                    dto.Labels.Add(monthDate.ToString("MMM yy"));

                    dto.Expenses.Add(expenses.Where(e => e.Date.Month == monthDate.Month && e.Date.Year == monthDate.Year).Sum(e => e.Amount));
                    dto.Incomes.Add(incomes.Where(i => i.Date.Month == monthDate.Month && i.Date.Year == monthDate.Year).Sum(i => i.Amount));
                }
            }
            return dto;
        }

        public async Task<BalanceTrendDto> GetBalanceTrendAsync(string userId, int months = 6)
        {
            var dto = new BalanceTrendDto();
            var allExpenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId);
            var allIncomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId);

            if (months == 1)
            {
                // BALANCE SEMANAL
                for (int i = 3; i >= 0; i--)
                {
                    var weekEnd = DateTime.Now.Date.AddDays(-(i * 7));
                    dto.Labels.Add($"Hasta {weekEnd:dd MMM}");

                    var accumulatedInc = allIncomes.Where(inc => inc.Date <= weekEnd).Sum(inc => inc.Amount);
                    var accumulatedExp = allExpenses.Where(e => e.Date <= weekEnd).Sum(e => e.Amount);

                    dto.Balances.Add(accumulatedInc - accumulatedExp);
                }
            }
            else
            {
                // BALANCE MENSUAL
                for (int i = months - 1; i >= 0; i--)
                {
                    var monthDate = DateTime.Now.AddMonths(-i);
                    var endOfMonth = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month));
                    dto.Labels.Add(monthDate.ToString("MMM yy"));

                    var accumulatedInc = allIncomes.Where(inc => inc.Date <= endOfMonth).Sum(inc => inc.Amount);
                    var accumulatedExp = allExpenses.Where(e => e.Date <= endOfMonth).Sum(e => e.Amount);

                    dto.Balances.Add(accumulatedInc - accumulatedExp);
                }
            }
            return dto;
        }

        public async Task<CategoryComparisonDto> GetCategoryComparisonAsync(string userId, int months = 6)
        {
            DateTime startDate = months == 1
                ? DateTime.Now.AddDays(-30)
                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-months + 1);

            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= startDate);
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var catMap = categories.ToDictionary(c => c.Id, c => c.Name);

            var grouped = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new { CategoryName = catMap.TryGetValue(g.Key, out var name) ? name : "General", Total = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Total).ToList();

            return new CategoryComparisonDto
            {
                Labels = grouped.Select(g => g.CategoryName).ToList(),
                Data = grouped.Select(g => g.Total).ToList()
            };
        }
    }
}
