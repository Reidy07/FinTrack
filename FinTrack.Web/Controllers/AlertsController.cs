
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class AlertsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Alertas";
            return View();
        }
    }
}
