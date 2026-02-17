using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IFinancialService _financialService;

        public CategoryController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categories = await _financialService.GetCategoriesByUserAsync(userId);
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _financialService.AddCategoryAsync(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }
    }
}
