using FinTrack.Core.DTOs.Category;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public CategoriesController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(string userId)
        {
            var categories = await _financialService.GetCategoriesByUserAsync(userId);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id, [FromQuery] string userId)
        {
            var category = await _financialService.GetCategoryByIdAsync(id, userId);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<CategoryDetailDto>> GetCategoryDetails(int id, [FromQuery] string userId)
        {
            var details = await _financialService.GetCategoryDetailsAsync(id, userId);
            if (details == null) return NotFound("Categoría no encontrada o sin permisos.");

            return Ok(details);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto dto, [FromQuery] string userId)
        {
            var created = await _financialService.AddCategoryAsync(dto, userId);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto dto, [FromQuery] string userId)
        {
            if (id != dto.Id) return BadRequest();
            await _financialService.UpdateCategoryAsync(dto, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id, [FromQuery] string userId)
        {
            await _financialService.DeleteCategoryAsync(id, userId);
            return NoContent();
        }
    }
}
