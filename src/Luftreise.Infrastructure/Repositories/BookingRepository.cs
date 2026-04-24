using Luftreise.Application.Interfaces;
using Luftreise.Domain.Entities;
using Luftreise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Luftreise.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly LuftreiseDbContext _context;

    public BookingRepository(LuftreiseDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .Include(b => b.User)
            .Include(b => b.Passengers)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
                .ThenInclude(f => f.DepartureAirport)
            .Include(b => b.Flight)
                .ThenInclude(f => f.ArrivalAirport)
            .Include(b => b.Passengers)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<Booking?> GetByReferenceAsync(string reference)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .Include(b => b.Passengers)
            .FirstOrDefaultAsync(b => b.BookingReference == reference);
    }
  public async Task<Booking> CreateBookingAsync(Booking booking)
  {
    await _context.Bookings.AddAsync(booking);
    await _context.SaveChangesAsync();
    return booking;
  }

  public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

  public async Task DeleteAsync(Booking booking)
  {
    _context.Bookings.Remove(booking);
    await _context.SaveChangesAsync();
  }

  public async Task<Flight?> GetFlightByIdAsync(int flightId)
  {
    return await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
  }
}
