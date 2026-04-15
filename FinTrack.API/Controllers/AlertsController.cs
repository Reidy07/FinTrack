using FinTrack.Core.Constants;
using FinTrack.Core.DTOs.Alerts;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public AlertsController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlerts([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ErrorMessages.UserIdRequired);

            var alerts = await _financialService.GetAlertsAsync(userId);
            return Ok(alerts);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ErrorMessages.UserIdRequired);

            var count = await _financialService.GetUnreadAlertCountAsync(userId);
            return Ok(count);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ErrorMessages.UserIdRequired);

            await _financialService.MarkAlertAsReadAsync(id, userId);
            return NoContent();
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ErrorMessages.UserIdRequired);

            await _financialService.MarkAllAlertsAsReadAsync(userId);
            return NoContent();
        }
    }
}
