using FinTrack.Core.DTOs.Expenses;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public ExpensesController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses(string userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var expenses = await _financialService.GetExpensesByUserAsync(userId, startDate, endDate);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetExpense(int id, [FromQuery] string userId)
        {
            var expense = await _financialService.GetExpenseByIdAsync(id, userId);
            if (expense == null) return NotFound("Gasto no encontrado");

            return Ok(expense);
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] ExpenseDto dto, [FromQuery] string userId)
        {
            var created = await _financialService.AddExpenseAsync(dto, userId);
            return Ok(created); // Retornamos 200 OK con el DTO creado
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseDto dto, [FromQuery] string userId)
        {
            if (id != dto.Id) return BadRequest("El ID del gasto no coincide.");

            await _financialService.UpdateExpenseAsync(dto, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id, [FromQuery] string userId)
        {
            await _financialService.DeleteExpenseAsync(id, userId);
            return NoContent();
        }
    }
}
