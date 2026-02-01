using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
