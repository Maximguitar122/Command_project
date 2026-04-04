using Microsoft.AspNetCore.Mvc;

namespace Luftreise_Command_project_.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();  
        }

        public IActionResult Sing_UP()
        {
            return View();
        }
    }
}
