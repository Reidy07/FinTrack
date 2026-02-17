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

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var incomes = await _financialService.GetIncomesByUserAsync(userId, null, null);
            return View(incomes);
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categories = await _financialService.GetCategoriesByUserAsync(userId);

            // Filtramos solo categorías de Gasto o Ambas
            ViewBag.Categories = new SelectList(
                categories.Where(c => c.Type == Core.Enum.CategoryType.Expense || c.Type == Core.Enum.CategoryType.Both),
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Si falla, hay que recargar la lista, si no la vista explota
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var categories = await _financialService.GetCategoriesByUserAsync(userId);
                ViewBag.Categories = new SelectList(
                    categories.Where(c => c.Type == Core.Enum.CategoryType.Expense || c.Type == Core.Enum.CategoryType.Both),
                    "Id",
                    "Name");

                return View(dto);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _financialService.AddExpenseAsync(dto, currentUserId);

            return RedirectToAction(nameof(Index));
        }
    }
}
