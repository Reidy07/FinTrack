using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class ExpenseController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Gastos";
            return View();
        }
    }
}
