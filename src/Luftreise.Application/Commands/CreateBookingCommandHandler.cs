using Luftreise.Application.DTOs;
using Luftreise.Application.Interfaces;
using Luftreise.Domain.Entities;
using MediatR;

namespace Luftreise.Application.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightRepository _flightRepository;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IFlightRepository flightRepository)
    {
        _bookingRepository = bookingRepository;
        _flightRepository = flightRepository;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var flight = await _flightRepository.GetByIdAsync(request.FlightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found");

        if (flight.AvailableSeats < request.NumberOfPassengers)
            throw new InvalidOperationException("Not enough available seats");

        var booking = new Booking
        {
            BookingReference = GenerateBookingReference(),
            FlightId = request.FlightId,
            UserId = request.UserId,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Pending,
            TotalPrice = flight.Price * request.NumberOfPassengers,
            NumberOfPassengers = request.NumberOfPassengers
        };

        await _bookingRepository.AddAsync(booking);

        flight.AvailableSeats -= request.NumberOfPassengers;
        await _flightRepository.UpdateAsync(flight);

        return new BookingDto
        {
            Id = booking.Id,
            BookingReference = booking.BookingReference,
            FlightId = booking.FlightId,
            BookingDate = booking.BookingDate,
            Status = booking.Status.ToString(),
            TotalPrice = booking.TotalPrice,
            NumberOfPassengers = booking.NumberOfPassengers
        };
    }

    private static string GenerateBookingReference()
    {
        return $"LR{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}
