namespace Luftreise.Application.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int FlightId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public int NumberOfPassengers { get; set; }
}
