using Luftreise.Application.Interfaces;
using Luftreise.Domain.Entities;
using Luftreise.Domain.Enums;
using Luftreise.Infrastructure.Data;
using Luftreise_Command_project_.Models;
using Luftreise_Command_project_.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Luftreise_Command_project_.Controllers;

public class AdminController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly LuftreiseDbContext _context;

    public AdminController(
        IUserRepository userRepository,
        IFlightRepository flightRepository,
        LuftreiseDbContext context)
    {
        _userRepository = userRepository;
        _flightRepository = flightRepository;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var admin = await GetAdminUserAsync();
        if (admin == null)
            return RedirectToAction("Login", "Account");

        var flights = await _flightRepository.GetAllAsync();
        var users = await _userRepository.GetAllAsync();

        var model = new AdminDashboardViewModel
        {
            TotalUsers = users.Count(),
            TotalFlights = flights.Count(),
            ActiveFlights = flights.Count(f => f.Status != FlightStatus.Cancelled),
            CancelledFlights = flights.Count(f => f.Status == FlightStatus.Cancelled)
        };

        return View("DashBoard", model);
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var admin = await GetAdminUserAsync();
        if (admin == null)
            return RedirectToAction("Login", "Account");

        var users = await _userRepository.GetAllAsync();
        var model = users
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

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Flights()
    {
        var admin = await GetAdminUserAsync();
        if (admin == null)
            return RedirectToAction("Login", "Account");

        var flights = await _flightRepository.GetAllAsync();
        return View(flights.OrderBy(f => f.DepartureTime).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> CreateFlight()
    {
        var admin = await GetAdminUserAsync();
        if (admin == null)
            return RedirectToAction("Login", "Account");

        await PopulateAirportsAsync();
        return View(new AdminFlightFormViewModel
        {
            DepartureTime = DateTime.Now.AddDays(1),
            ArrivalTime = DateTime.Now.AddDays(1).AddHours(2),
            AvailableSeats = 20,
            TotalSeats = 30,
            FlightClass = "Economy",
            AirlineName = "Luftreise Airways"
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlight(AdminFlightFormViewModel model)
    {
        var admin = await GetAdminUserAsync();
        if (admin == null)
            return RedirectToAction("Login", "Account");

        if (model.DepartureAirportId == model.ArrivalAirportId)
            ModelState.AddModelError(nameof(model.ArrivalAirportId), "Аеропорти вильоту і прильоту мають відрізнятися.");

        if (model.ArrivalTime <= model.DepartureTime)
            ModelState.AddModelError(nameof(model.ArrivalTime), "Час прильоту має бути пізніше за час вильоту.");

        var existingFlight = await _context.Flights
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FlightNumber == model.FlightNumber);

        if (existingFlight != null)
            ModelState.AddModelError(nameof(model.FlightNumber), "Рейс із таким номером уже існує.");

        if (!ModelState.IsValid)
        {
            await PopulateAirportsAsync();
            return View(model);
        }

        var flight = new Flight
        {
            FlightNumber = model.FlightNumber.Trim(),
            AirlineName = model.AirlineName.Trim(),
            DepartureAirportId = model.DepartureAirportId,
            ArrivalAirportId = model.ArrivalAirportId,
            DepartureTime = model.DepartureTime,
            ArrivalTime = model.ArrivalTime,
            Price = model.Price,
            AvailableSeats = model.AvailableSeats,
            TotalSeats = model.TotalSeats,
            FlightClass = model.FlightClass.Trim(),
            Status = FlightStatus.Scheduled
        };

        await _flightRepository.AddAsync(flight);
        TempData["SuccessMessage"] = "Рейс додано";
        return RedirectToAction(nameof(Flights));
    }

    [HttpPost]
    public async Task<IActionResult> CancelFlight(int id, bool returnToProfile = false)
    {
        var admin = await GetAdminUserAsync();
        if (admin == null)
            return RedirectToAction("Login", "Account");

        var flight = await _flightRepository.GetByIdAsync(id);
        if (flight == null)
            return returnToProfile
                ? RedirectToAction("Profile", "Account")
                : RedirectToAction(nameof(Flights));

        flight.Status = FlightStatus.Cancelled;
        await _flightRepository.UpdateAsync(flight);
        TempData["SuccessMessage"] = "Рейс скасовано";
        return returnToProfile
            ? RedirectToAction("Profile", "Account")
            : RedirectToAction(nameof(Flights));
    }

    private async Task<Luftreise.Domain.Entities.User?> GetAdminUserAsync()
    {
        var sessionEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(sessionEmail))
            return null;

        var user = await _userRepository.GetByEmailAsync(sessionEmail);
        if (user == null || user.Role != UserRole.Admin)
            return null;

        HttpContext.Session.SetString("UserIsAdmin", "Admin");
        return user;
    }

    private async Task PopulateAirportsAsync()
    {
        var airports = await _context.Airports
            .AsNoTracking()
            .OrderBy(a => a.City)
            .Select(a => new
            {
                a.Id,
                Label = a.City + " (" + a.Code + ")"
            })
            .ToListAsync();

        ViewBag.Airports = new SelectList(airports, "Id", "Label");
    }
}
