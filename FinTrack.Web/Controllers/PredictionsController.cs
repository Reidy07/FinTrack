using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class PredictionsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Predicciones";
            return View();
        }
    }
}
