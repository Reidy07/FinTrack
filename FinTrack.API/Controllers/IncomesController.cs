using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomesController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public IncomesController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<IncomeDto>>> GetIncomes(string userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var incomes = await _financialService.GetIncomesByUserAsync(userId, startDate, endDate);
            return Ok(incomes);
        }

        [HttpPost]
        public async Task<ActionResult<IncomeDto>> CreateIncome([FromBody] IncomeDto dto, [FromQuery] string userId)
        {
            var created = await _financialService.AddIncomeAsync(dto, userId);
            return Ok(created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IncomeDto>> GetIncome(int id, [FromQuery] string userId)
        {
            var income = await _financialService.GetIncomeByIdAsync(id, userId);
            if (income == null) return NotFound();

            return Ok(income);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncome(int id, [FromBody] IncomeDto dto, [FromQuery] string userId)
        {
            var dtoId = dto.Id;

            if (id != dtoId)
                return BadRequest("El Id de la URL no coincide con el del objeto.");

            await _financialService.UpdateIncomeAsync(dto, userId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id, [FromQuery] string userId)
        {
            await _financialService.DeleteIncomeAsync(id, userId);
            return NoContent();
        }
    }
}