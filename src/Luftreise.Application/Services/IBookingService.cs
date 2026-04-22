using Luftreise.Application.DTOs;

namespace Luftreise.Application.Services;

public interface IBookingService
{
  Task<BookingDto> CreateBookingAsync(int flightId, int userId, int numberOfPassengers);
}
