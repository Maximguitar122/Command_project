using Luftreise.Application.Interfaces;
using Luftreise.Domain.Entities;
using Luftreise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Luftreise.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly LuftreiseDbContext _context;

    public FlightRepository(LuftreiseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string departureCity, string arrivalCity, DateTime departureDate)
    {
        return await _context.Flights
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Where(f => f.DepartureAirport.City == departureCity
                     && f.ArrivalAirport.City == arrivalCity
                     && f.DepartureTime.Date == departureDate.Date
                     && f.AvailableSeats > 0)
            .OrderBy(f => f.DepartureTime)
            .ToListAsync();
    }

    public async Task<Flight?> GetByIdAsync(int id)
    {
        return await _context.Flights
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Flight>> GetAllAsync()
    {
        return await _context.Flights
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .ToListAsync();
    }

    public async Task AddAsync(Flight flight)
    {
        await _context.Flights.AddAsync(flight);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Flight flight)
    {
        _context.Flights.Update(flight);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var flight = await _context.Flights.FindAsync(id);
        if (flight != null)
        {
            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
        }
    }
}
