using FinTrack.Core.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Obtener datos desde la API
            var summary = await _httpClient.GetFromJsonAsync<DashboardSummaryDto>($"api/dashboard/summary?userId={userId}");

            // 2. Obtener predicción desde la API
            var prediction = await _httpClient.GetFromJsonAsync<decimal>($"api/dashboard/prediction?userId={userId}");

            ViewBag.Prediction = prediction;

            return View(summary);
        }
    }
}
