using FinTrack.Core.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>($"api/categories/user/{userId}");
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();

            // Llamamos a nuestra API
            var details = await _httpClient.GetFromJsonAsync<CategoryDetailDto>($"api/categories/{id}/details?userId={userId}");

            if (details == null) return NotFound();

            return View(details); // Enviamos el DTO a la vista
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var userId = GetUserId();

            dto.Description ??= string.Empty; // Si viene nulo, lo hace un texto vacío ""
            dto.Color = string.IsNullOrWhiteSpace(dto.Color) ? "#3498db" : dto.Color;

            ModelState.Remove(nameof(dto.Description));
            ModelState.Remove(nameof(dto.Color));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Faltan datos obligatorios (Nombre o Tipo).";
                return RedirectToAction(nameof(Index));
            }

            var response = await _httpClient.PostAsJsonAsync($"api/categories?userId={userId}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Categoría creada correctamente.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error al crear: {error}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDto dto)
        {
            var userId = GetUserId();

            dto.Description ??= string.Empty;
            dto.Color = string.IsNullOrWhiteSpace(dto.Color) ? "#3498db" : dto.Color;

            ModelState.Remove(nameof(dto.Description));
            ModelState.Remove(nameof(dto.Color));

            var response = await _httpClient.PutAsJsonAsync($"api/categories/{dto.Id}?userId={userId}", dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo actualizar la categoría.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            await _httpClient.DeleteAsync($"api/categories/{id}?userId={userId}");
            return RedirectToAction(nameof(Index));
        }
    }
}

