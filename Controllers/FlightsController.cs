using Luftreise.Application.DTOs;
using Luftreise.Application.Interfaces;
using Luftreise.Application.Services;
using Luftreise.Infrastructure.Data;
using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Luftreise_Command_project.Controllers
{
  public class FlightsController : Controller
  {
    private readonly IFlightService _flightService;
    private readonly IBookingService _bookingService;
    private readonly IBookingRepository _bookingRepository;
    private readonly LuftreiseDbContext _context;

    public FlightsController(
      IFlightService flightService,
      IBookingService bookingService,
      IBookingRepository bookingRepository,
      LuftreiseDbContext context)
    {
      _flightService = flightService;
      _bookingService = bookingService;
      _bookingRepository = bookingRepository;
      _context = context;
    }

    [HttpGet]
    public IActionResult Flights()
    {
      return View(new List<FlightDto>());
    }

    [HttpGet]
    public IActionResult Booking(
      int id,
      string airline,
      string flightNumber,
      string fromCity,
      string toCity,
      string departureTime,
      string arrivalTime,
      DateTime flightDate,
      string flightClass,
      double price,
      int passengers = 1)
    {
      var userEmail = HttpContext.Session.GetString("UserEmail");
      var passengerCount = Math.Clamp(passengers, 1, 9);

      var model = new BookingViewModel
      {
        FlightId = id,
        Airline = airline,
        FlightNumber = flightNumber,
        FromCity = fromCity,
        ToCity = toCity,
        DepartureTime = departureTime,
        ArrivalTime = arrivalTime,
        FlightDate = flightDate,
        FlightClass = flightClass,
        Price = price * passengerCount,
        Passengers = Enumerable.Range(0, passengerCount)
          .Select(index => new PassengerFormViewModel
          {
            Email = index == 0 ? userEmail ?? string.Empty : string.Empty
          })
          .ToList()
      };

      if (string.IsNullOrWhiteSpace(model.Airline) || model.Price <= 0)
        PopulateBookingModel(model);

      return View(model);
    }

    [HttpGet]
    public IActionResult AirportPayment(BookingViewModel model)
    {
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Booking(BookingViewModel model)
    {
      model.Passengers ??= new List<PassengerFormViewModel>();
      PopulateBookingModel(model);

      ModelState.Remove(nameof(BookingViewModel.Price));
      ModelState.Remove(nameof(BookingViewModel.FlightDate));
      ModelState.Remove(nameof(BookingViewModel.Airline));
      ModelState.Remove(nameof(BookingViewModel.FlightNumber));
      ModelState.Remove(nameof(BookingViewModel.FromCity));
      ModelState.Remove(nameof(BookingViewModel.ToCity));
      ModelState.Remove(nameof(BookingViewModel.DepartureTime));
      ModelState.Remove(nameof(BookingViewModel.ArrivalTime));
      ModelState.Remove(nameof(BookingViewModel.FlightClass));

      if (!ModelState.IsValid)
        return View(model);

      var userEmail = HttpContext.Session.GetString("UserEmail");
      if (string.IsNullOrEmpty(userEmail))
        return RedirectToAction("Login", "Account");

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
      if (user == null)
        return RedirectToAction("Login", "Account");

      var passengers = model.Passengers
        .Where(p => !string.IsNullOrWhiteSpace(p.FirstName)
                 || !string.IsNullOrWhiteSpace(p.LastName)
                 || !string.IsNullOrWhiteSpace(p.PassportNumber)
                 || !string.IsNullOrWhiteSpace(p.Email))
        .ToList();

      if (!passengers.Any())
      {
        ModelState.AddModelError(string.Empty, "Додайте хоча б одного пасажира.");
        return View(model);
      }

      var bookingDto = await _bookingService.CreateBookingAsync(model.FlightId, user.Id, passengers.Count);
      var booking = await _bookingRepository.GetByIdAsync(bookingDto.Id);

      if (booking == null)
        return RedirectToAction("Profile", "Account");

      booking.SeatNumber = string.Join(", ", passengers
        .Select(p => p.SeatNumber?.Trim())
        .Where(seat => !string.IsNullOrWhiteSpace(seat)));
      booking.NumberOfPassengers = passengers.Count;
      booking.Passengers.Clear();

      foreach (var passenger in passengers)
      {
        booking.Passengers.Add(new Luftreise.Domain.Entities.Passenger
        {
          FirstName = passenger.FirstName.Trim(),
          LastName = passenger.LastName.Trim(),
          PassportNumber = passenger.PassportNumber.Trim(),
          DateOfBirth = new DateTime(2000, 1, 1),
          Nationality = "Україна"
        });
      }

      await _bookingRepository.UpdateAsync(booking);
      model.BookingReference = bookingDto.BookingReference;
      model.Price = (double)bookingDto.TotalPrice;

      return View("AirportPayment", model);
    }

    [HttpPost]
    public async Task<IActionResult> Search(SearchModels model)
    {
      var searchDto = new FlightSearchDto
      {
        DepartureCity = NormalizeAirportInput(model.From),
        ArrivalCity = NormalizeAirportInput(model.To),
        DepartureDate = model.FlightDate
      };

      var flights = await _flightService.SearchFlightsAsync(searchDto);
      return View("Flights", flights);
    }

    [HttpGet]
    public async Task<IActionResult> SearchAirports(string term)
    {
      if (string.IsNullOrWhiteSpace(term))
        return Json(new List<object>());

      var normalizedTerm = term.Trim().ToLowerInvariant();

      var airport = (await _context.Airports
        .AsNoTracking()
        .ToListAsync())
        .Select(a => new
        {
          id = a.Id,
          city = GetDisplayCity(a.City),
          code = a.Code,
          name = a.Name,
          text = GetDisplayCity(a.City) + " (" + a.Code + ")",
          search = BuildAirportSearchText(a.City, a.Code, a.Name)
        })
        .Where(a => a.search.Contains(normalizedTerm))
        .Take(10)
        .Select(a => new
        {
          a.id,
          a.city,
          a.code,
          a.name,
          a.text
        })
        .ToList();

      return Json(airport);
    }

    [HttpPost]
    public async Task<IActionResult> CancelBooking(int id)
    {
      var userEmail = HttpContext.Session.GetString("UserEmail");
      if (string.IsNullOrEmpty(userEmail))
        return RedirectToAction("Login", "Account");

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
      if (user == null)
        return RedirectToAction("Login", "Account");

      var booking = await _bookingRepository.GetByIdAsync(id);
      if (booking == null || booking.UserId != user.Id)
        return RedirectToAction("Profile", "Account");

      await _bookingRepository.DeleteAsync(booking);
      TempData["SuccessMessage"] = "Бронювання скасовано";
      return RedirectToAction("Profile", "Account");
    }

    private static string NormalizeAirportInput(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return string.Empty;

      var normalized = value.Trim();
      var bracketIndex = normalized.IndexOf(" (", StringComparison.Ordinal);

      if (bracketIndex >= 0)
        normalized = normalized[..bracketIndex];

      return TranslateCityToDatabaseName(normalized.Trim());
    }

    private static string TranslateCityToDatabaseName(string city)
    {
      return city.ToLowerInvariant() switch
      {
        "київ" => "Kyiv",
        "львів" => "Lviv",
        "прага" => "Prague",
        "варшава" => "Warsaw",
        "будапешт" => "Budapest",
        _ => city
      };
    }

    private static string GetDisplayCity(string city)
    {
      return city switch
      {
        "Kyiv" => "Київ",
        "Lviv" => "Львів",
        "Prague" => "Прага",
        "Warsaw" => "Варшава",
        "Budapest" => "Будапешт",
        _ => city
      };
    }

    private static string BuildAirportSearchText(string city, string code, string name)
    {
      var displayCity = GetDisplayCity(city);

      var aliases = city switch
      {
        "Kyiv" => "київ kyiv",
        "Lviv" => "львів lviv",
        "Prague" => "прага prague",
        "Warsaw" => "варшава warsaw",
        "Budapest" => "будапешт budapest",
        _ => city.ToLowerInvariant()
      };

      return string.Join(" ", new[]
      {
        city,
        displayCity,
        code,
        name,
        aliases
      }).ToLowerInvariant();
    }

    private void PopulateBookingModel(BookingViewModel model)
    {
      if (model.FlightId <= 0)
        return;

      var flight = _context.Flights
        .AsNoTracking()
        .Include(f => f.DepartureAirport)
        .Include(f => f.ArrivalAirport)
        .FirstOrDefault(f => f.Id == model.FlightId);

      if (flight == null)
        return;

      model.Airline = flight.AirlineName;
      model.FlightNumber = flight.FlightNumber;
      model.FromCity = flight.DepartureAirport?.City ?? model.FromCity;
      model.ToCity = flight.ArrivalAirport?.City ?? model.ToCity;
      model.DepartureTime = flight.DepartureTime.ToString("HH:mm");
      model.ArrivalTime = flight.ArrivalTime.ToString("HH:mm");
      model.FlightDate = flight.DepartureTime.Date;
      model.FlightClass = flight.FlightClass;
      model.Price = (double)flight.Price * Math.Max(model.Passengers.Count, 1);
    }

  }
}
