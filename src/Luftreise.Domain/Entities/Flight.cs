namespace Luftreise.Domain.Entities;

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public int DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = null!;
    public int ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public string AirlineName { get; set; } = string.Empty;
    public FlightStatus Status { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
