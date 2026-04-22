using Luftreise.Application.DTOs;

namespace Luftreise.Application.Services;

public interface IFlightService
{
  Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchDto searchDto);
}
