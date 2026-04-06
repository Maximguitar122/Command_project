using Microsoft.AspNetCore.Mvc;

namespace Luftreise_Command_project_.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();  
        }
        [HttpGet]
        public IActionResult Sign_Up()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sign_UP(
            string FullName,
             string Email,
             string Phone,
             string City,
             string Country,
             DateTime? BirthDate,
             string Password,
             string ConfirmPassword)
        {
            TempData["SuccessMessage"] = "Ви успішно зареєструвались!";
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string Email)
        {
            TempData["SuccessMessage"] = "Інструкції надіслані на email!";
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
