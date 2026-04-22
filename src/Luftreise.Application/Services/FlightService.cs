using Luftreise.Application.DTOs;
using Luftreise.Application.Interfaces;

namespace Luftreise.Application.Services;

public class FlightService : IFlightService
{
  private readonly IFlightRepository _flightRepository;

  public FlightService(IFlightRepository flightRepository)
  {
    _flightRepository = flightRepository;
  }

  public async Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchDto searchDto)
  {
    var flights = await _flightRepository.SearchFlightsAsync(
        searchDto.DepartureCity,
        searchDto.ArrivalCity,
        searchDto.DepartureDate);

    return flights.Select(f => new FlightDto
    {
      Id = f.Id,
      FlightNumber = f.FlightNumber,
      AirlineName = f.AirlineName,
      DepartureTime = f.DepartureTime,
      ArrivalTime = f.ArrivalTime,
      Price = f.Price,
      AvailableSeats = f.AvailableSeats,
      Status = f.Status.ToString(),
      DepartureAirport = f.DepartureAirport?.Name ?? "",
      ArrivalAirport = f.ArrivalAirport?.Name ?? ""
    });
  }
}
