namespace Luftreise.Application.DTOs;

public class FlightDto
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string AirlineName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string FlightClass { get; set; } = string.Empty;  
}
