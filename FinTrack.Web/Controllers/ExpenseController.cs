using FinTrack.Core.DTOs;
using FinTrack.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinTrack.Web.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IFinancialService _financialService;

        public ExpenseController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity!.Name!;
            var expenses = await _financialService.GetExpensesAsync(userId);
            return View(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            dto.UserId = User.Identity!.Name!;
            await _financialService.AddExpenseAsync(dto);
            return RedirectToAction(nameof(Index));
        }
    }
}