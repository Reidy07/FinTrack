using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IFinancialService _financialService;

        public ExpenseController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task LoadExpenseCategoriesAsync(string userId)
        {
            var categories = await _financialService.GetCategoriesByUserAsync(userId);

            ViewBag.Categories = new SelectList(
                categories.Where(c =>
                    c.Type == Core.Enum.CategoryType.Expense ||
                    c.Type == Core.Enum.CategoryType.Both),
                "Id",
                "Name"
            );
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var expenses = await _financialService.GetExpensesByUserAsync(userId, null, null);
            await LoadExpenseCategoriesAsync(userId);

            return View(expenses);
        }

        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            await LoadExpenseCategoriesAsync(userId);
            return View(new ExpenseDto { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            if (!ModelState.IsValid)
            {
                await LoadExpenseCategoriesAsync(userId);
                return View(dto);
            }

            await _financialService.AddExpenseAsync(dto, userId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var expense = await _financialService.GetExpenseByIdAsync(id, userId);
            if (expense == null) return NotFound();

            await LoadExpenseCategoriesAsync(userId);
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpenseDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            if (!ModelState.IsValid)
            {
                await LoadExpenseCategoriesAsync(userId);
                return View(dto);
            }

            await _financialService.UpdateExpenseAsync(dto, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            await _financialService.DeleteExpenseAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
