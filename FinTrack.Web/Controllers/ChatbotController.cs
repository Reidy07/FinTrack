using FinTrack.Core.DTOs.Chatbot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class ChatbotController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChatbotController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
        {
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var response = await _httpClient.PostAsJsonAsync("api/chatbot/ask", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
                return Json(new { success = true, reply = result?.Reply });
            }

            return Json(new { success = false, reply = "Error al conectar con la API." });
        }
    }
}