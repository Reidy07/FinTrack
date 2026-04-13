using FinTrack.Core.Constants;
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
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>($"api/categories/user/{userId}");
            if (categories != null)
            {
                ViewBag.Categories = new SelectList(categories.Where(c => c.Type == Core.Enum.CategoryType.Expense || c.Type == Core.Enum.CategoryType.Both), "Id", "Name");
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
                TempData[TempDataKeys.Warning] = ErrorMessages.RequiredField;
                await LoadExpenseCategoriesAsync(userId);
                return View(dto);
            }

            var response = await _httpClient.PostAsJsonAsync($"api/expenses?userId={userId}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData[TempDataKeys.Success] = SuccessMessages.Created;
                return RedirectToAction(nameof(Index));
            }

            TempData[TempDataKeys.Error] = ErrorMessages.ApiSaveError;
            await LoadExpenseCategoriesAsync(userId);
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var expense = await _httpClient.GetFromJsonAsync<ExpenseDto>($"api/expenses/{id}?userId={userId}");

            if (expense == null)
            {
                TempData[TempDataKeys.Error] = ErrorMessages.NotFound;
                return RedirectToAction(nameof(Index));
            }

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
                TempData[TempDataKeys.Warning] = ErrorMessages.RequiredField;
                await LoadExpenseCategoriesAsync(userId);
                return View(dto);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/expenses/{dto.Id}?userId={userId}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData[TempDataKeys.Success] = SuccessMessages.Updated;
                return RedirectToAction(nameof(Index));
            }

            TempData[TempDataKeys.Error] = ErrorMessages.ApiSaveError;
            await LoadExpenseCategoriesAsync(userId);
            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var response = await _httpClient.DeleteAsync($"api/expenses/{id}?userId={userId}");

            if (response.IsSuccessStatusCode)
                TempData[TempDataKeys.Success] = SuccessMessages.Deleted;
            else
                TempData[TempDataKeys.Error] = ErrorMessages.ApiSaveError;

            return RedirectToAction(nameof(Index));
        }
    }
}
