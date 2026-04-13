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
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest(ErrorMessages.UserIdRequired);

            var alerts = await _financialService.GetAlertsAsync(userId);
            return Ok(alerts);
        }

        // Endpoint para marcar una alerta como leída
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest(ErrorMessages.UserIdRequired);

            await _financialService.MarkAlertAsReadAsync(id, userId);
            return Ok();
        }
    }
}