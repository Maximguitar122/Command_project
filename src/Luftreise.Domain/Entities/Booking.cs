namespace Luftreise.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int FlightId { get; set; }
    public Flight Flight { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime BookingDate { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int NumberOfPassengers { get; set; }
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}
