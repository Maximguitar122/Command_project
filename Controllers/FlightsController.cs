using Luftreise_Command_project_.Data;
using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Mvc;
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
             //   Price = price
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

            return RedirectToAction("Pay", model); 

        }

    }
} 