using FinTrack.Core.DTOs;
using FinTrack.Core.DTOs.Category;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;

namespace FinTrack.Infrastructure.Services
{
    public class GeminiPredictionService : IGeminiPredictionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GeminiPredictionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeminiPredictionResultDto> GetFinancialPredictionAsync(
            string userId,
            List<string> userPriorities,
            decimal? savingsGoalPercentage = null)
        {
            var now = DateTime.Now;
            var threeMonthsAgo = now.AddMonths(-3);

            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId && e.Date >= threeMonthsAgo);
            var incomes = await _unitOfWork.Incomes.FindAsync(i => i.UserId == userId && i.Date >= threeMonthsAgo);
            var categories = await _unitOfWork.Categories.FindAsync(c => true);

            var totalExpenses = expenses.Sum(e => e.Amount);
            var totalIncome = incomes.Sum(i => i.Amount);
            var balance = totalIncome - totalExpenses;

            var avgMonthlyExpenses = totalExpenses / 3;
            var avgMonthlyIncome = totalIncome / 3;

            // Tendencias mensuales
            var monthlyTrends = new List<MonthlyTrendDto>();
            for (int i = 2; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var mExp = expenses.Where(e => e.Date.Month == month.Month && e.Date.Year == month.Year).Sum(e => e.Amount);
                var mInc = incomes.Where(inc => inc.Date.Month == month.Month && inc.Date.Year == month.Year).Sum(inc => inc.Amount);
                monthlyTrends.Add(new MonthlyTrendDto
                {
                    Month = month.ToString("MMMM yyyy"),
                    Income = mInc,
                    Expenses = mExp
                });
            }

            // Prediccion proximo mes
            var predictedNextMonth = CalculatePredictedExpenses(monthlyTrends, avgMonthlyExpenses);

            // Meta de ahorro
            var savingsPct = savingsGoalPercentage.HasValue ? savingsGoalPercentage.Value / 100m : 0.20m;
            var recommendedSavings = avgMonthlyIncome * savingsPct;
            var freeToSpend = Math.Max(0, avgMonthlyIncome - predictedNextMonth - recommendedSavings);

            // Estado financiero
            var expenseRatio = avgMonthlyIncome > 0 ? avgMonthlyExpenses / avgMonthlyIncome : 1;
            var healthStatus = expenseRatio < 0.80m ? "Good" : expenseRatio < 1.00m ? "Warning" : "Critical";

            // Top categorias
            var topCategories = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g =>
                {
                    var cat = categories.FirstOrDefault(c => c.Id == g.Key);
                    var amount = g.Sum(e => e.Amount);
                    return new CategorySummaryDto
                    {
                        CategoryName = cat?.Name ?? "Sin categoría",
                        Amount = amount,
                        Percentage = totalExpenses > 0 ? Math.Round(amount / totalExpenses * 100, 1) : 0
                    };
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToList();

            var recommendations = GenerateRecommendations(avgMonthlyIncome, avgMonthlyExpenses, expenseRatio, topCategories, userPriorities, monthlyTrends);
            var categoryAdvice = GenerateCategoryAdvice(topCategories, avgMonthlyIncome);
            var summary = GenerateSummary(healthStatus, avgMonthlyIncome, avgMonthlyExpenses, balance, expenseRatio);

            return new GeminiPredictionResultDto
            {
                Summary = summary,
                PredictedNextMonthExpenses = Math.Round(predictedNextMonth, 2),
                RecommendedSavings = Math.Round(Math.Max(0, recommendedSavings), 2),
                FreeToSpend = Math.Round(freeToSpend, 2),
                HealthStatus = healthStatus,
                Recommendations = recommendations,
                PaymentReminders = GeneratePaymentReminders(expenses, categories),
                CategoryAdvice = categoryAdvice
            };
        }

        private decimal CalculatePredictedExpenses(List<MonthlyTrendDto> trends, decimal average)
        {
            if (trends.Count < 2) return average;
            var first = trends.First().Expenses;
            var last = trends.Last().Expenses;
            var trend = (last - first) / Math.Max(1, trends.Count - 1);
            var predicted = last + trend;
            return Math.Max(0, (predicted * 0.6m) + (average * 0.4m));
        }

        private List<string> GenerateRecommendations(
            decimal avgIncome, decimal avgExpenses, decimal expenseRatio,
            List<CategorySummaryDto> topCategories,
            List<string> userPriorities, List<MonthlyTrendDto> trends)
        {
            var recs = new List<string>();

            if (expenseRatio >= 1.0m)
                recs.Add("Tus gastos superan tus ingresos. Identifica gastos no esenciales que puedas eliminar inmediatamente.");
            else if (expenseRatio >= 0.80m)
                recs.Add("Tus gastos representan más del 80% de tus ingresos. Busca reducir al menos un 10% en gastos variables.");
            else
                recs.Add($"Buen control financiero. Estás gastando el {(expenseRatio * 100):F0}% de tus ingresos mensuales.");

            if (trends.Count >= 2)
            {
                var lastTwo = trends.TakeLast(2).ToList();
                if (lastTwo[1].Expenses > lastTwo[0].Expenses * 1.10m)
                    recs.Add("Tus gastos aumentaron más del 10% el último mes. Revisa en qué categorías gastaste más.");
                else if (lastTwo[1].Expenses < lastTwo[0].Expenses * 0.90m)
                    recs.Add("¡Excelente! Redujiste tus gastos el último mes. Mantén ese hábito de ahorro.");
            }

            if (topCategories.Any())
            {
                var top = topCategories.First();
                if (top.Percentage > 40)
                    recs.Add($"La categoría '{top.CategoryName}' representa el {top.Percentage}% de tus gastos. Considera establecer un límite mensual.");
            }

            if (userPriorities.Any(p => p.ToLower().Contains("ahorro") || p.ToLower().Contains("emergencia")))
                recs.Add("Para tu meta de ahorro, automatiza una transferencia fija el día que recibes tu ingreso.");

            if (userPriorities.Any(p => p.ToLower().Contains("deuda")))
                recs.Add("Para pagar deudas más rápido, aplica el método avalancha: primero la deuda con mayor tasa de interés.");

            if (userPriorities.Any(p => p.ToLower().Contains("inver")))
                recs.Add("Considera invertir en fondos indexados de bajo costo una vez tengas un fondo de emergencia de 3 meses.");

            return recs.Take(5).ToList();
        }

        private List<CategoryAdviceDto> GenerateCategoryAdvice(List<CategorySummaryDto> topCategories, decimal avgMonthlyIncome)
        {
            var advice = new List<CategoryAdviceDto>();
            foreach (var cat in topCategories)
            {
                var recommendedPct = cat.CategoryName.ToLower() switch
                {
                    var n when n.Contains("aliment") || n.Contains("comida") => 0.15m,
                    var n when n.Contains("transport") || n.Contains("auto") => 0.15m,
                    var n when n.Contains("entretenim") || n.Contains("ocio") => 0.05m,
                    var n when n.Contains("ropa") || n.Contains("vestim") => 0.05m,
                    var n when n.Contains("salud") || n.Contains("medic") => 0.10m,
                    var n when n.Contains("educac") => 0.10m,
                    _ => 0.10m
                };

                var recommendedMax = avgMonthlyIncome * recommendedPct;
                var monthlySpending = cat.Amount / 3;
                var isOverBudget = monthlySpending > recommendedMax;

                advice.Add(new CategoryAdviceDto
                {
                    Category = cat.CategoryName,
                    CurrentSpending = Math.Round(monthlySpending, 2),
                    RecommendedMax = Math.Round(recommendedMax, 2),
                    Advice = isOverBudget
                        ? $"Excedes en ${monthlySpending - recommendedMax:F2} el límite recomendado. Intenta reducir este gasto."
                        : $"Buen manejo. Tienes ${recommendedMax - monthlySpending:F2} de margen disponible."
                });
            }
            return advice;
        }

        private List<PaymentReminderDto> GeneratePaymentReminders(
            IEnumerable<FinTrack.Core.Entities.Expense> expenses,
            IEnumerable<FinTrack.Core.Entities.Category> categories)
        {
            var reminders = new List<PaymentReminderDto>();
            var recurring = expenses.Where(e => e.IsRecurring).ToList();
            foreach (var exp in recurring.Take(3))
            {
                var cat = categories.FirstOrDefault(c => c.Id == exp.CategoryId);
                reminders.Add(new PaymentReminderDto
                {
                    Name = string.IsNullOrEmpty(exp.Description) ? cat?.Name ?? "Pago recurrente" : exp.Description,
                    DueDate = "Próximo mes",
                    EstimatedAmount = exp.Amount
                });
            }
            return reminders;
        }

        private string GenerateSummary(string healthStatus, decimal avgIncome, decimal avgExpenses, decimal balance, decimal ratio)
        {
            var statusText = healthStatus switch
            {
                "Good" => "Tu situación financiera es saludable.",
                "Warning" => "Tu situación financiera requiere atención.",
                "Critical" => "Tu situación financiera está en estado crítico.",
                _ => "Análisis de tu situación financiera."
            };
            var balanceText = balance >= 0
                ? $"En los últimos 3 meses generaste un balance positivo de ${balance:F2}."
                : $"En los últimos 3 meses tuviste un déficit de ${Math.Abs(balance):F2}.";
            var ratioText = $"Estás destinando el {ratio * 100:F0}% de tus ingresos a gastos, con un promedio mensual de ${avgExpenses:F2} en gastos y ${avgIncome:F2} en ingresos.";
            return $"{statusText} {balanceText} {ratioText}";
        }
    }
}