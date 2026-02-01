using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Iniciar sesión";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Registro";
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            ViewData["Title"] = "Perfil";
            return View();
        }
    }
}
