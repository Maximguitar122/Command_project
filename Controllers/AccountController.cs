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

                string uniqueFileName = Guid.NewGuid().ToString() + extension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.AvatarFile.CopyTo(stream);
                }

                model.AvatarPath = "/uploads/avatars/" + uniqueFileName;
            }

            model.Id = UserStore.Users.Count > 0 ? UserStore.Users.Max(x => x.Id) + 1 : 1;
            model.IsAdmin = false;

            UserStore.Users.Add(model);

            TempData["SuccessMessage"] = "Реєстрація успішна";
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var currentUser = UserStore.CurrentUser;
            if (currentUser == null)
                return RedirectToAction("Login");

            return View(currentUser);
        }

        [HttpPost]
        public IActionResult EditProfile(User model)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserStore.GetUserByEmail(sessionEmail);

            if (user == null)
                return RedirectToAction("Login");

            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(model.AvatarFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Дозволені лише файли jpg, jpeg, png, webp");
                    return View(model);
                }

                if (model.AvatarFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Файл не повинен бути більшим за 5 МБ");
                    return View(model);
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + extension;
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Ви вийшли з акаунта";
            return RedirectToAction("Login");
        }
    }
}