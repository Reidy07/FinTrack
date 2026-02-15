using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            // Usamos ClaimTypes.NameIdentifier para obtener el ID (Guid)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expenses = await _financialService.GetExpensesByUserAsync(userId, null, null);

            return View(expenses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            //  Obtenemos el ID de nuevo
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // El método AddExpenseAsync ahora pide (dto, userId)
            await _financialService.AddExpenseAsync(dto, userId);

            return RedirectToAction(nameof(Index));
        }
    }
}