using Luftreise.Application.DTOs;
using MediatR;

namespace Luftreise.Application.Commands;

public record CreateBookingCommand(
    int FlightId,
    int UserId,
    int NumberOfPassengers
) : IRequest<BookingDto>;
