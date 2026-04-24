using Luftreise.Application.Interfaces;
using Luftreise.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Luftreise_Command_project_.Models;
using System.Security.Cryptography;
using System.Text;
using Luftreise.Domain.Enums;

namespace Luftreise_Command_project_.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            IUserRepository userRepository,
            IBookingRepository bookingRepository,
            IFlightRepository flightRepository,
            IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _environment = environment;

        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userRepository.GetByEmailAsync(model.Email);

            if (user == null || user.PasswordHash != HashPassword(model.Password))
            {
                ModelState.AddModelError("", "Неправильний email або пароль");
                return View(model);
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserIsAdmin", user.Role == UserRole.Admin ? "Admin" : "User");

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
        public async Task<IActionResult> Sign_Up(Luftreise_Command_project_.Models.User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Користувач з таким email уже існує");
                return View(model);
            }

            string? avatarPath = null;

            if (model.AvatarFile != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                string extension = Path.GetExtension(model.AvatarFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Дозволені лише файли: jpg, jpeg, png, webp");
                    return View(model);
                }

                if (model.AvatarFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Розмір файлу не повинен перевищувати 5 МБ");
                    return View(model);
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.AvatarFile.CopyToAsync(stream);

                avatarPath = "/uploads/avatars/" + uniqueFileName;
            }

            var names = (model.FullName ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var firstName = names.Length > 0 ? names[0] : string.Empty;
            var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : string.Empty;

            var user = new Luftreise.Domain.Entities.User
            {
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = model.Phone ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User,
                
                City = model.City,
                Country = model.Country,
                BirthDate = model.BirthDate,
                AvatarPath = avatarPath

            };

            await _userRepository.AddAsync(user);

            TempData["SuccessMessage"] = "Реєстрація успішна";
            return RedirectToAction("Login");
        }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
      var sessionEmail = HttpContext.Session.GetString("UserEmail");

      if (string.IsNullOrEmpty(sessionEmail))
        return RedirectToAction("Login");

      var currentUser = await _userRepository.GetByEmailAsync(sessionEmail);

      if (currentUser == null)
        return RedirectToAction("Login");

      var model = new Luftreise_Command_project_.Models.User
      {
        Id = currentUser.Id,
        Email = currentUser.Email,
        FullName = $"{currentUser.FirstName} {currentUser.LastName}".Trim(),
        Phone = currentUser.PhoneNumber,
        City = currentUser.City,
        Country = currentUser.Country,
        BirthDate = currentUser.BirthDate,
        AvatarPath = currentUser.AvatarPath,
        IsAdmin = currentUser.Role == UserRole.Admin
      };

      var userBookings = await _bookingRepository.GetByUserIdAsync(currentUser.Id);

      var tickets = userBookings.Select(b => new BookingTicket
      {
        Id = b.Id,
        BookingNumber = b.BookingReference,
        PassengerCount = b.Passengers?.Count > 0 ? b.Passengers.Count : Math.Max(1, b.NumberOfPassengers),
        PassengerNames = b.Passengers != null && b.Passengers.Any()
          ? string.Join(", ", b.Passengers.Select(p => $"{p.FirstName} {p.LastName}".Trim()))
          : $"{currentUser.FirstName} {currentUser.LastName}".Trim(),
        PassengerName = b.Passengers != null && b.Passengers.Any()
          ? string.Join(", ", b.Passengers.Select(p => $"{p.FirstName} {p.LastName}".Trim()))
          : $"{currentUser.FirstName} {currentUser.LastName}".Trim(),
        FlightNumber = b.Flight?.FlightNumber ?? "",
        Airline = b.Flight?.AirlineName ?? "",
        FromCity = b.Flight?.DepartureAirport?.City ?? "",
        ToCity = b.Flight?.ArrivalAirport?.City ?? "",
        FlightDate = b.Flight?.DepartureTime ?? DateTime.Now,
        Price = b.TotalPrice,
        Status = b.Status.ToString(),
        SeatNumber = b.SeatNumber
      }).ToList();

      ViewBag.Bookings = tickets;
      ViewBag.Tickets = tickets;

      if (currentUser.Role == UserRole.Admin)
      {
        var users = await _userRepository.GetAllAsync();
        var flights = await _flightRepository.GetAllAsync();

        ViewBag.AdminUsers = users
          .OrderByDescending(u => u.CreatedAt)
          .Select(u => new Luftreise_Command_project_.Models.User
          {
            Id = u.Id,
            FullName = $"{u.FirstName} {u.LastName}".Trim(),
            Email = u.Email,
            Phone = u.PhoneNumber,
            City = u.City ?? string.Empty,
            Country = u.Country ?? string.Empty,
            BirthDate = u.BirthDate,
            AvatarPath = u.AvatarPath,
            IsAdmin = u.Role == UserRole.Admin
          })
          .ToList();

        ViewBag.AdminFlights = flights
          .OrderBy(f => f.DepartureTime)
          .ToList();
      }

      return View(model);
    }

    [HttpPost]
        public async Task<IActionResult> DeleteUser(int id, bool returnToProfile = false)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var currentUser = await _userRepository.GetByEmailAsync(sessionEmail);
            if (currentUser == null || currentUser.Role != UserRole.Admin)
                return RedirectToAction("Profile");

            if (currentUser.Id == id)
            {
                TempData["SuccessMessage"] = "Адмін не може видалити сам себе";
                return returnToProfile
                    ? RedirectToAction("Profile")
                    : RedirectToAction("Users", "Admin");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
                await _userRepository.DeleteAsync(user);

            TempData["SuccessMessage"] = "Користувача видалено";
            return returnToProfile
                ? RedirectToAction("Profile")
                : RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var currentUser = await _userRepository.GetByEmailAsync(sessionEmail);
            if (currentUser == null || currentUser.Role != UserRole.Admin)
                return RedirectToAction("Profile");

            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var user = await _userRepository.GetByEmailAsync(sessionEmail);
            if (user == null)
                return RedirectToAction("Login");

            await _userRepository.DeleteAsync(user);

            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserEmail");
            TempData["SuccessMessage"] = "Акаунт видалено";
            return RedirectToAction("Login");
        }

        
        [HttpPost]
        public async Task<IActionResult> EditProfile(Luftreise_Command_project_.Models.User model)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("Login");

            var user = await _userRepository.GetByEmailAsync(sessionEmail);
            if (user == null)
                return RedirectToAction("Login");

            ModelState.Remove("Email");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
                return View("Profile", model);

            var names = (model.FullName ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            user.FirstName = names.Length > 0 ? names[0] : string.Empty;
            user.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : string.Empty;
            user.PhoneNumber = model.Phone ?? string.Empty;
            user.City = model.City ?? string.Empty;
            user.Country = model.Country ?? string.Empty;
            user.BirthDate = model.BirthDate;
           if(model.AvatarFile != null)
           {   
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
            string extension = Path.GetExtension(model.AvatarFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
             ModelState.AddModelError("AvatarFile", "Дозволені лише файли: jpg, jpeg, png, webp");
             return View("Profile", model);
            }

            if (model.AvatarFile.Length > 5 * 1024 * 1024)
           {
            ModelState.AddModelError("AvatarFile", "Розмір файлу не повинен перевищувати 5 МБ");
            return View("Profile", model);
            }

              string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
              Directory.CreateDirectory(uploadsFolder);

               string uniqueFileName = Guid.NewGuid() + extension;
               string filePath = Path.Combine(uploadsFolder, uniqueFileName);

               using var stream = new FileStream(filePath, FileMode.Create);
               await model.AvatarFile.CopyToAsync(stream);

               user.AvatarPath = "/uploads/avatars/" + uniqueFileName;

            }
 

      await _userRepository.UpdateAsync(user);

            TempData["SuccessMessage"] = "Профіль оновлено!";
            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserEmail");
            TempData["SuccessMessage"] = "Ви вийшли з акаунта";
            return RedirectToAction("Login");
        }
    }
}
