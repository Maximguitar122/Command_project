using Luftreise.Application.DTOs;
using Luftreise.Application.Interfaces;
using Luftreise.Domain.Entities;
using Luftreise.Domain.Enums;

namespace Luftreise.Application.Services;

public class BookingService : IBookingService
{
  private readonly IBookingRepository _bookingRepository;

  public BookingService(IBookingRepository bookingRepository)
  {
    _bookingRepository = bookingRepository;
  }

  public async Task<BookingDto> CreateBookingAsync(int flightId, int userId, int numberOfPassengers)
  {
    var booking = new Booking
    {
      FlightId = flightId,
      UserId = userId,
      NumberOfPassengers = numberOfPassengers,
      BookingDate = DateTime.UtcNow,
      Status = BookingStatus.Confirmed
    };

    var createdBooking = await _bookingRepository.CreateBookingAsync(booking);

    return new BookingDto
    {
      Id = createdBooking.Id,
      FlightId = createdBooking.FlightId,
      BookingDate = createdBooking.BookingDate,
      Status = createdBooking.Status.ToString(),
      NumberOfPassengers = createdBooking.NumberOfPassengers,
      TotalPrice = createdBooking.TotalPrice,
      BookingReference = createdBooking.BookingReference
    };
  }
}
