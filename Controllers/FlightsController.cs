using Luftreise.Application.DTOs;
using Luftreise.Application.Services;
using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Luftreise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Luftreise.Application.Interfaces;

namespace Luftreise_Command_project.Controllers
{
  public class FlightsController : Controller
  {
    private readonly IFlightService _flightService;
    private readonly IBookingService _bookingService;
    private readonly LuftreiseDbContext _context;

    public FlightsController(IFlightService flightService, IBookingService bookingService)
    {
      _flightService = flightService;
      _bookingService = bookingService;
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
        double price)
    {
      var userEmail = HttpContext.Session.GetString("UserEmail");

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
        Email = userEmail,
        Price = price
      };

      return View(model);
    }

    [HttpGet]
    public IActionResult AirportPayment(BookingViewModel model)
    {
      return View(model);
    }

    [HttpPost]
    public IActionResult Booking(BookingViewModel model)
    {
      if (!ModelState.IsValid)
        return View(model);

      var userEmail = HttpContext.Session.GetString("UserEmail");

      if (string.IsNullOrEmpty(userEmail))
        return RedirectToAction("Login", "Account");

      return View("AirportPayment", model);
    }

    [HttpPost]
    public async Task<IActionResult> Search(SearchModels model)
    {
      var searchDto = new FlightSearchDto
      {
        DepartureCity = model.From,
        ArrivalCity = model.To,
        DepartureDate = model.FlightDate
      };

      var flights = await _flightService.SearchFlightsAsync(searchDto);

      return View("Flights", flights);
    }

    [HttpGet]
    public async Task<IActionResult> SearchAirports(string term)
    {
      if (string.IsNullOrEmpty(term))
      {
        return Json(new List<object>());
      }
      var airport = await _context.Airports.Where
      (a => a.City.Contains(term)
      || a.Code.Contains(term)
      || a.Name.Contains(term))
      .Select(a => new
      {
        id = a.Id,
        city = a.City,
        code = a.Code,
        name = a.Name,
        text = a.City + " (" + a.Code + ")"
      })
      .Take(10)
      .ToListAsync();
      return Json(airport);
    }
  }
}
