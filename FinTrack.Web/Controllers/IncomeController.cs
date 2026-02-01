using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class IncomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Ingresos";
            return View();
        }
    }
}
