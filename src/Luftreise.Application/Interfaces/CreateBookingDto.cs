namespace Luftreise.Application.DTOs;

public class CreateBookingDto
{
  public int FlightId { get; set; }
  public string Email { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string PassportNumber { get; set; } = string.Empty;
  public string FlightClass { get; set; } = string.Empty;
  public double Price { get; set; }
}
