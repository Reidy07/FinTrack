using FinTrack.Core.Constants;
using FinTrack.Core.DTOs.Category;
using FinTrack.Core.DTOs.Reports;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("period-comparison")]
        public async Task<ActionResult<PeriodComparisonDto>> GetPeriodComparison([FromQuery] string userId, [FromQuery] int months = 6)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest(ErrorMessages.UserIdRequired);

            var result = await _reportService.GetPeriodComparisonAsync(userId, months);
            return Ok(result);
        }

        [HttpGet("balance-trend")]
        public async Task<ActionResult<BalanceTrendDto>> GetBalanceTrend([FromQuery] string userId, [FromQuery] int months = 6)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest(ErrorMessages.UserIdRequired);

            var result = await _reportService.GetBalanceTrendAsync(userId, months);
            return Ok(result);
        }
        [HttpGet("category-comparison")]
        public async Task<ActionResult<CategoryComparisonDto>> GetCategoryComparison([FromQuery] string userId, [FromQuery] int months = 6)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest(ErrorMessages.UserIdRequired);

            var result = await _reportService.GetCategoryComparisonAsync(userId, months);
            return Ok(result);
        }
    }
}
