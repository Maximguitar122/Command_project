using Luftreise.Application.DTOs;
using Luftreise.Domain.Entities;

namespace Luftreise.Application.Services;

public interface IBookingService
{
  Task<BookingDto> CreateBookingAsync(int flightId, int userId, int numberOfPassengers);
  
}
