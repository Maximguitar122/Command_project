using NetTopologySuite.Geometries;

namespace Luftreise.Domain.Entities;

public class Airport
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public Point? Location { get; set; }
    public ICollection<Flight> DepartureFlights { get; set; } = new List<Flight>();
    public ICollection<Flight> ArrivalFlights { get; set; } = new List<Flight>();
}
