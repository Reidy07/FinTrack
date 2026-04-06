using FinTrack.Core.Interfaces.Services;
using FinTrack.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IFinancialService _financialService;
        private readonly IPredictionService _predictionService;

        public DashboardController(
            IFinancialService financialService,
            IPredictionService predictionService)
        {
            _financialService = financialService;
            _predictionService = predictionService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            var referenceDate = DateTime.Now;
            var startOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            var summary = await _financialService.GetDashboardSummaryAsync(userId, referenceDate);
            var prediction = await _predictionService.PredictNextMonthExpensesAsync(userId);
            var currentBalance = await _financialService.GetCurrentBalanceAsync(userId);

            var expensesThisMonth = await _financialService.GetExpensesByUserAsync(userId, startOfMonth, endOfMonth);
            var incomesThisMonth = await _financialService.GetIncomesByUserAsync(userId, startOfMonth, endOfMonth);

            var model = new DashboardViewModel
            {
                Summary = summary,
                PredictedNextMonthExpenses = prediction,
                CurrentBalance = currentBalance,
                TransactionsThisMonth = expensesThisMonth.Count() + incomesThisMonth.Count(),
                ReferenceDate = referenceDate
            };

            return View(model);
        }
    }
}