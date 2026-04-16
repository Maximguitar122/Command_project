using Luftreise.Application.DTOs;
using MediatR;

namespace Luftreise.Application.Queries;

public record SearchFlightsQuery(
    string DepartureCity,
    string ArrivalCity,
    DateTime DepartureDate
) : IRequest<IEnumerable<FlightDto>>;
