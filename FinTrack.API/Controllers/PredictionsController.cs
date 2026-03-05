using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/predictions")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly IGeminiPredictionService _predictionService;

        public PredictionsController(IGeminiPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<GeminiPredictionResultDto>> Analyze([FromBody] PredictionAnalyzeRequestDto request)
        {
            try
            {
                var result = await _predictionService.GetFinancialPredictionAsync(
                    request.UserId,
                    request.Priorities,
                    request.SavingsGoalPercentage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class PredictionAnalyzeRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Priorities { get; set; } = new();
        public decimal? SavingsGoalPercentage { get; set; }
    }
}