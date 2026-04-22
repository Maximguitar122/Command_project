using Luftreise.Application.DTOs;
using Luftreise.Application.Interfaces;
using MediatR;

namespace Luftreise.Application.Queries;

public class SearchFlightsQueryHandler : IRequestHandler<SearchFlightsQuery, IEnumerable<FlightDto>>
{
    private readonly IFlightRepository _flightRepository;

    public SearchFlightsQueryHandler(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<IEnumerable<FlightDto>> Handle(SearchFlightsQuery request, CancellationToken cancellationToken)
    {
        var flights = await _flightRepository.SearchFlightsAsync(
            request.DepartureCity,
            request.ArrivalCity,
            request.DepartureDate
        );

        return flights.Select(f => new FlightDto
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            DepartureAirport = f.DepartureAirport.Name,
            ArrivalAirport = f.ArrivalAirport.Name,
            DepartureTime = f.DepartureTime,
            ArrivalTime = f.ArrivalTime,
            Price = f.Price,
            AvailableSeats = f.AvailableSeats,
            AirlineName = f.AirlineName,
            Status = f.Status.ToString()
        });
    }
}
