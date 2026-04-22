namespace Luftreise.Application.DTOs;

public class FlightSearchDto
{
    public string DepartureCity { get; set; } = string.Empty;
    public string ArrivalCity { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public int Passengers { get; set; } = 1;
}
