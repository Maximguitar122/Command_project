using System;


namespace Luftreise_Command_project_.Models
{
    public class BookingTicket
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public DateTime FlightDate { get; set; }
        public string FlightClass { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int PassengerCount { get; set; }
        public string PassengerNames { get; set; } = string.Empty;
        public double BaggageWeight { get; set; }
        public string PassportNumber { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = "A12";
        public string Status { get; set; } = "Підтверджено";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
