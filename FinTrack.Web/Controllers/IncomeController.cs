using FinTrack.Core.DTOs.Category;
using FinTrack.Core.DTOs.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public IncomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task LoadIncomeCategoriesAsync(string userId)
        {
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>($"api/categories/user/{userId}");

            if (categories != null)
            {
                ViewBag.Categories = new SelectList(
                    categories.Where(c => c.Type == Core.Enum.CategoryType.Income || c.Type == Core.Enum.CategoryType.Both),
                    "Id", "Name");
            }
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var incomes = await _httpClient.GetFromJsonAsync<IEnumerable<IncomeDto>>($"api/incomes/user/{userId}");

            await LoadIncomeCategoriesAsync(userId);
            return View(incomes);
        }

        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            await LoadIncomeCategoriesAsync(userId);
            return View(new IncomeDto { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeDto dto)
        {
            var userId = GetUserId();
            ModelState.Remove(nameof(dto.CategoryName));

            if (!ModelState.IsValid)
            {
                await LoadIncomeCategoriesAsync(userId);
                return View(dto);
            }

            var response = await _httpClient.PostAsJsonAsync($"api/incomes?userId={userId}", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al guardar el ingreso.");
            await LoadIncomeCategoriesAsync(userId);
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var income = await _httpClient.GetFromJsonAsync<IncomeDto>($"api/incomes/{id}?userId={userId}");
            if (income == null) return NotFound();

            await LoadIncomeCategoriesAsync(userId);
            return View(income);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IncomeDto dto)
        {
            var userId = GetUserId();
            ModelState.Remove(nameof(dto.CategoryName));

            if (!ModelState.IsValid)
            {
                await LoadIncomeCategoriesAsync(userId);
                return View(dto);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/incomes/{dto.Id}?userId={userId}", dto);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar.");
            await LoadIncomeCategoriesAsync(userId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            await _httpClient.DeleteAsync($"api/incomes/{id}?userId={userId}");

            return RedirectToAction(nameof(Index));
        }
    }
}
