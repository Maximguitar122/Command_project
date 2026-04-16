using Luftreise.Domain.Entities;

namespace Luftreise.Application.Interfaces;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> SearchFlightsAsync(string departureCity, string arrivalCity, DateTime departureDate);
    Task<Flight?> GetByIdAsync(int id);
    Task<IEnumerable<Flight>> GetAllAsync();
    Task AddAsync(Flight flight);
    Task UpdateAsync(Flight flight);
    Task DeleteAsync(int id);
}
