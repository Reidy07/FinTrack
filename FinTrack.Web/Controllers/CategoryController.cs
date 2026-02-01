using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Categorías";
            return View();
        }
    }
}
