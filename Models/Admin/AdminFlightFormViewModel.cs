using System.ComponentModel.DataAnnotations;

namespace Luftreise_Command_project_.Models.Admin;

public class AdminFlightFormViewModel
{
    [Required]
    public string FlightNumber { get; set; } = string.Empty;

    [Required]
    public string AirlineName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int DepartureAirportId { get; set; }

    [Range(1, int.MaxValue)]
    public int ArrivalAirportId { get; set; }

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    [Range(1, 100000)]
    public decimal Price { get; set; }

    [Range(0, 1000)]
    public int AvailableSeats { get; set; }

    [Range(1, 1000)]
    public int TotalSeats { get; set; }

    [Required]
    public string FlightClass { get; set; } = string.Empty;
}
