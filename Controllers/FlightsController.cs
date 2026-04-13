using Luftreise_Command_project_.Data;
using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Mvc;

namespace Luftreise_Command_project_.Controllers
{
    public class FlightsController : Controller
    {
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
                Price = price,
                SeatNumber = "A12"
            };

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

            var selectedSeat = string.IsNullOrWhiteSpace(model.SeatNumber)
                ? "A12"
                : model.SeatNumber;

            var ticket = new BookingTicket
            {
                BookingNumber = "LR-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                UserEmail = userEmail,
                PassengerName = $"{model.FirstName} {model.LastName}",
                Airline = model.Airline,
                FlightNumber = model.FlightNumber,
                FromCity = model.FromCity,
                ToCity = model.ToCity,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime,
                FlightDate = model.FlightDate,
                FlightClass = model.FlightClass,
                Price = model.Price,
                BaggageWeight = model.BaggageWeight,
                PassportNumber = model.PassportNumber,
                SeatNumber = selectedSeat,
                Status = "Підтверджено"
            };

            BookingStore.AddBooking(ticket);

            return RedirectToAction("AirportPayment", new
            {
                model.FirstName,
                model.LastName,
                model.Airline,
                model.FlightNumber,
                model.FromCity,
                model.ToCity,
                model.DepartureTime,
                model.ArrivalTime,
                model.FlightDate,
                model.FlightClass,
                model.Price,
                model.BaggageWeight,
                SeatNumber = selectedSeat
            });
        }

        [HttpGet]
        public IActionResult AirportPayment(BookingViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SeatNumber))
                model.SeatNumber = "A12";

            return View(model);
        }

        [HttpGet]
        public IActionResult BookingDetails(int id)
        {
            var ticket = BookingStore.GetById(id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        [HttpPost]
        public IActionResult CancelBooking(int id)
        {
            BookingStore.CancelBooking(id);
            return RedirectToAction("Profile", "Account");
        }
    }
} 