using Luftreise.Application.Interfaces;

using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Mvc;

namespace Luftreise_Command_project.Controllers
{ 
    public class FlightsController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightRepository _flightRepository;
    

    
    public FlightsController(IBookingRepository bookingRepository, IFlightRepository flightRepository)
        {
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
      
    }

        [HttpGet]
        public IActionResult Flights()
        {
            return View();
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
              var flights = await _flightRepository.SearchFlightsAsync(model.From,
              model.To,
              model.FlightDate);
            return View("Flights", flights);
        }
    }
}
