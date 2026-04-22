using Luftreise.Domain.Entities;

namespace Luftreise.Application.Interfaces;

public interface IBookingRepository
{
  Task<Booking?> GetByIdAsync(int id);
  Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
  Task<Booking?> GetByReferenceAsync(string reference);

  Task<Booking> CreateBookingAsync(Booking booking);

  Task AddAsync(Booking booking);
  Task UpdateAsync(Booking booking);
}
