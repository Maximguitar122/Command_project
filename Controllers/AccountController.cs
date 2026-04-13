using Microsoft.AspNetCore.Mvc;
using Luftreise_Command_project_.Models;
using Luftreise_Command_project_.Data;

namespace Luftreise_Command_project_.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public AccountController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }






        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }





        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = UserStore.GetUser(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Неправильний email або пароль");
                return View(model);
            }

            HttpContext.Session.SetString("UserEmail", user.Email);

            if (model.RememberMe)
            {
                Response.Cookies.Append("UserEmail", user.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                });
            }
            else
            {
                Response.Cookies.Delete("UserEmail");
            }

            TempData["SuccessMessage"] = "Вхід успішний";
            return RedirectToAction("Profile");
        }

        
        
        
        
        
        [HttpGet]
        public IActionResult Sign_Up()
        {
            return View();
        }

        
        
        
        [HttpPost]
        public IActionResult Sign_UP(User model)
        {
            if (!ModelState.IsValid)
                return View("Sign_Up", model);

            if (UserStore.Users.Any(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "Користувач з таким email уже існує");
                return View("Sign_Up", model);
            }

            if (model.AvatarFile != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(model.AvatarFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Дозволені лише файли: jpg, jpeg, png, webp");
                    return View("Sign_Up", model);
                }

                if (model.AvatarFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Розмір файлу не повинен перевищувати 5 МБ");
                    return View("Sign_Up", model);
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.AvatarFile.CopyTo(stream);
                }

                model.AvatarPath = "/uploads/avatars/" + uniqueFileName;
            }

            model.Id = UserStore.GetNextId();
            model.IsAdmin = false;

            UserStore.AddUser(model);

            TempData["SuccessMessage"] = "Реєстрація успішна";
            return RedirectToAction("Login");
        }

       

        
        
        
        
        [HttpPost]
        public IActionResult EditProfile(User model)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var user = UserStore.GetUserByEmail(sessionEmail);
            if (user == null)
                return RedirectToAction("Login");
            ModelState.Remove("Email");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
                return View("Profile", user);

            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(model.AvatarFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Дозволені лише файли jpg, jpeg, png, webp");
                    return View("Profile", user);
                }

                if (model.AvatarFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Файл не повинен бути більшим за 5 МБ");
                    return View("Profile", user);
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.AvatarFile.CopyTo(stream);
                }

                user.AvatarPath = "/uploads/avatars/" + uniqueFileName;
            }

            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.City = model.City;
            user.Country = model.Country;
            user.BirthDate = model.BirthDate;

            TempData["SuccessMessage"] = "Профіль оновлено!";
            return RedirectToAction("Profile");
        }

        
        
        
        
        
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var user = UserStore.GetUserByEmail(sessionEmail);
            if (user == null)
                return RedirectToAction("Login");

            UserStore.RemoveUser(user);
            HttpContext.Session.Clear();
            UserStore.CurrentUser = null;

            TempData["SuccessMessage"] = "Акаунт видалено";
            return RedirectToAction("Login");
        }

       
        
        
        
        
        
        [HttpGet]
        public IActionResult Users()
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var currentUser = UserStore.GetUserByEmail(sessionEmail);
            if (currentUser == null || !currentUser.IsAdmin)
                return RedirectToAction("Profile");

            return View(UserStore.GetAllUsers());
        }

        
        
        
        
        
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var currentUser = UserStore.GetUserByEmail(sessionEmail);
            if (currentUser == null || !currentUser.IsAdmin)
                return RedirectToAction("Profile");

            if (currentUser.Id == id)
            {
                TempData["SuccessMessage"] = "Адмін не може видалити сам себе";
                return RedirectToAction("Users");
            }

            UserStore.RemoveUserById(id);
            TempData["SuccessMessage"] = "Користувача видалено";
            return RedirectToAction("Users");
        }

        
        
        
        
        
        
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserEmail");
            UserStore.CurrentUser = null;
            TempData["SuccessMessage"] = "Ви вийшли з акаунта";
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Profile()
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var currentUser = UserStore.GetUserByEmail(sessionEmail);

            var tickets = BookingStore.GetTicketsByEmail(sessionEmail);
            ViewBag.Tickets = tickets;

            return View(currentUser);
        }
    }
}