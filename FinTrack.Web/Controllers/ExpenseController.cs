using FinTrack.Core.DTOs.Category;
using FinTrack.Core.DTOs.Expenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly HttpClient _httpClient;

        public ExpenseController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinTrackAPI");
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task LoadExpenseCategoriesAsync(string userId)
        {
            // Llamamos a la API de categorías
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>($"api/categories/user/{userId}");

            if (categories != null)
            {
                ViewBag.Categories = new SelectList(
                    categories.Where(c => c.Type == Core.Enum.CategoryType.Expense || c.Type == Core.Enum.CategoryType.Both),
                    "Id", "Name");
            }
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var expenses = await _httpClient.GetFromJsonAsync<IEnumerable<ExpenseDto>>($"api/expenses/user/{userId}");
            await LoadExpenseCategoriesAsync(userId);

            return View(expenses);
        }

        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            await LoadExpenseCategoriesAsync(userId);
            return View(new ExpenseDto { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            var userId = GetUserId();

            dto.UserId = userId;

            ModelState.Remove(nameof(dto.UserId));
            ModelState.Remove(nameof(dto.CategoryName));

            if (!ModelState.IsValid)
            {
                await LoadExpenseCategoriesAsync(userId);
                return View(dto);
            }

            // POST a la API
            var response = await _httpClient.PostAsJsonAsync($"api/expenses?userId={userId}", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al guardar en la API.");
            await LoadExpenseCategoriesAsync(userId);
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            // Pedimos el gasto a la API
            var expense = await _httpClient.GetFromJsonAsync<ExpenseDto>($"api/expenses/{id}?userId={userId}");

            if (expense == null) return NotFound();

            await LoadExpenseCategoriesAsync(userId);
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpenseDto dto)
        {
            var userId = GetUserId();
            dto.UserId = userId;

            ModelState.Remove(nameof(dto.UserId));
            ModelState.Remove(nameof(dto.CategoryName));

            if (!ModelState.IsValid)
            {
                await LoadExpenseCategoriesAsync(userId);
                return View(dto);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/expenses/{dto.Id}?userId={userId}", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar el gasto.");
            await LoadExpenseCategoriesAsync(userId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            // DELETE a la API
            await _httpClient.DeleteAsync($"api/expenses/{id}?userId={userId}");
            return RedirectToAction(nameof(Index));
        }
    }
}
