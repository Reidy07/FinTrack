using FinTrack.Core.DTOs.Dashboard;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IFinancialService _financialService;
        private readonly IPredictionService _predictionService;

        public DashboardController(IFinancialService financialService, IPredictionService predictionService)
        {
            _financialService = financialService;
            _predictionService = predictionService;
        }
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest("Falta el UserId");

            var summary = await _financialService.GetDashboardSummaryAsync(userId, DateTime.Now);
            return Ok(summary);
        }

        [HttpGet("prediction")]
        public async Task<ActionResult<decimal>> GetNextMonthPrediction([FromQuery] string userId)
        {
            var prediction = await _predictionService.PredictNextMonthExpensesAsync(userId);
            return Ok(prediction);
        }
    }
}