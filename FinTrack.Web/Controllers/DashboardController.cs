using FinTrack.Core.Interfaces.Services;
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

        public DashboardController(IFinancialService financialService, IPredictionService predictionService)
        {
            _financialService = financialService;
            _predictionService = predictionService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            // 1. Obtener datos reales
            var summary = await _financialService.GetDashboardSummaryAsync(userId, DateTime.Now);

            // 2. Obtener predicción
            var prediction = await _predictionService.PredictNextMonthExpensesAsync(userId);

            ViewBag.Prediction = prediction;

            return View(summary);
        }
    }
}
