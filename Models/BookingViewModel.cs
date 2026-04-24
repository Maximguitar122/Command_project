using System.ComponentModel.DataAnnotations;
namespace Luftreise_Command_project_.Models
{
    public class BookingViewModel
    {
        public int FlightId { get; set; } 
        public string BookingReference { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public DateTime FlightDate { get; set; }
        public string FlightClass { get; set; } = string.Empty;
        public double Price { get; set; }
        public List<PassengerFormViewModel> Passengers { get; set; } = new()
        {
            new PassengerFormViewModel()
        };
    }
}
