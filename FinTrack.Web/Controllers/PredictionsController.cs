using FinTrack.Core.Constants;
using FinTrack.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class PredictionsController : Controller
    {
        private readonly HttpClient _httpClient;

        public PredictionsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] PredictionFormDto form)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, error = "Usuario no autenticado." });

                // Usamos un objeto anónimo con la estructura exacta que espera la API
                var request = new
                {
                    UserId = userId,
                    Priorities = form.Priorities ?? new List<string>(),
                    SavingsGoalPercentage = form.SavingsGoalPercentage
                };

                // Hacemos la petición a nuestra API
                var response = await _httpClient.PostAsJsonAsync("api/predictions/analyze", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GeminiPredictionResultDto>();
                    return Json(new { success = true, data = result });
                }

                return Json(new { success = false, error = ErrorMessages.ApiSaveError });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }

    public class PredictionFormDto
    {
        public List<string>? Priorities { get; set; }
        public decimal? SavingsGoalPercentage { get; set; }
    }
}
