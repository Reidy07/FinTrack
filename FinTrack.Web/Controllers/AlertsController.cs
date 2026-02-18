using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class AlertsController : Controller
{
    private readonly IFinancialService _financialService;

    public AlertsController(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var alerts = await _financialService.GetAlertsAsync(userId);
        return View(alerts);
    }

}