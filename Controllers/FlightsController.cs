using Luftreise_Command_project_.Data;
using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace Luftreise_Command_project_.Controllers
{
    public class FlightsController : Controller
    {
        [HttpGet]
        public IActionResult Flights()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(SearchModels model)
        {
            return View("Flights", model);
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
            decimal price)
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
                Price = price
            };

            return View("Booking", model);
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

            var ticket = new BookingTicket
            {
                Id = BookingStore.GetAllTickets().Count + 1,
                BookingNumber = "LR" + DateTime.Now.Ticks.ToString().Substring(10),
                UserEmail = userEmail,

                Airline = model.Airline,
                FlightNumber = model.FlightNumber,
                FromCity = model.FromCity,
                ToCity = model.ToCity,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime,
                FlightDate = model.FlightDate,
                FlightClass = model.FlightClass,
                Price = (double)model.Price,

                PassportNumber = model.PassportNumber,
                BaggageWeight = model.BaggageWeight
            };

            BookingStore.AddTicket(ticket);

            return View("AirportPayment", model);
        }

     


      
        







    }
}