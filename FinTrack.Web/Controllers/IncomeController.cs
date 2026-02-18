using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly IFinancialService _financialService;

        public IncomeController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task LoadIncomeCategoriesAsync(string userId)
        {
            var categories = await _financialService.GetCategoriesByUserAsync(userId);

            // Solo categorías de Ingreso o Ambas
            ViewBag.Categories = new SelectList(
                categories.Where(c =>
                    c.Type == Core.Enum.CategoryType.Income ||
                    c.Type == Core.Enum.CategoryType.Both),
                "Id",
                "Name"
            );
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var incomes = await _financialService.GetIncomesByUserAsync(userId, null, null);
            await LoadIncomeCategoriesAsync(userId);

            return View(incomes);
        }

        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            await LoadIncomeCategoriesAsync(userId);
            return View(new IncomeDto { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            if (!ModelState.IsValid)
            {
                await LoadIncomeCategoriesAsync(userId);
                return View(dto);
            }

            await _financialService.AddIncomeAsync(dto, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
