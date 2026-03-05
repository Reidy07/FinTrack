using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class PredictionsController : Controller
    {
        private readonly IGeminiPredictionService _predictionService;

        public PredictionsController(IGeminiPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] PredictionFormDto form)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, error = "Usuario no autenticado." });

                var result = await _predictionService.GetFinancialPredictionAsync(
                    userId,
                    form.Priorities ?? new List<string>(),
                    form.SavingsGoalPercentage);

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }

    public class PredictionFormDto
    {
        public List<string>? Priorities { get; set; }
        public decimal? SavingsGoalPercentage { get; set; }
    }
}