using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Profile()
        {
            ViewData["Title"] = "Perfil";
            return View();
        }
    }
}
