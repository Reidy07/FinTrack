using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IFinancialService _financialService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryController(IFinancialService financialService, UserManager<ApplicationUser> userManager)
        {
            _financialService = financialService;
            _userManager = userManager;
        }

        private string? UserId() => _userManager.GetUserId(User);

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();


            var categories = await _financialService.GetCategoriesByUserAsync(userId);
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var userId = UserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            if (!ModelState.IsValid)
            {
                var categories = await _financialService.GetCategoriesByUserAsync(userId);
                return View("Index", categories);
            }

            await _financialService.AddCategoryAsync(dto, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDto dto)
        {
            var userId = UserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            await _financialService.UpdateCategoryAsync(dto, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = UserId();
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            await _financialService.DeleteCategoryAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
