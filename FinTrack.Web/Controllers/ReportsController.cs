using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class ReportsController : Controller
{
    private readonly IFinancialService _financialService;

    public ReportsController(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    public IActionResult Index()
    {
        return View();
    }

    // Endpoint para Chart.js: Gastos vs Ingresos por mes
    [HttpGet]
    public async Task<IActionResult> GetMonthlyFlow()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();


        var expenses = await _financialService.GetExpensesByUserAsync(userId, DateTime.Now.AddMonths(-6), DateTime.Now);
        var incomes = await _financialService.GetIncomesByUserAsync(userId, DateTime.Now.AddMonths(-6), DateTime.Now);

        // Agrupar por mes
        var data = new
        {
            labels = expenses.Select(e => e.Date.ToString("MMMM")).Distinct(),
            expenseData = expenses.GroupBy(e => e.Date.Month).Select(g => g.Sum(e => e.Amount)),
            incomeData = incomes.GroupBy(i => i.Date.Month).Select(g => g.Sum(i => i.Amount))
        };

        return Json(data);
    }
}
