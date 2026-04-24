using Luftreise.Domain.Entities;
using Luftreise.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Security.Cryptography;
using System.Text;

namespace Luftreise.Infrastructure.Data
{
  public static class DbSeeder
  {
    public static async Task SeedAsync(LuftreiseDbContext context)
    {
      var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
      if (pendingMigrations.Any())
        await context.Database.MigrateAsync();

      await SeedAirportsAsync(context);
      await SeedFlightsAsync(context);
      await SeedAdminAsync(context);
    }

    private static async Task SeedAirportsAsync(LuftreiseDbContext context)
    {
      if (await context.Airports.AnyAsync())
        return;

      var airports = new List<Airport>
            {
                new Airport
                {
                    Code = "KBP",
                    Name = "Boryspil International Airport",
                    City = "Kyiv",
                    Country = "Ukraine",
                    Location = new Point(30.8947, 50.3450) { SRID = 4326 }
                },
                new Airport
                {
                    Code = "LWO",
                    Name = "Lviv Danylo Halytskyi International Airport",
                    City = "Lviv",
                    Country = "Ukraine",
                    Location = new Point(23.9561, 49.8125) { SRID = 4326 }
                },
                new Airport
                {
                    Code = "PRG",
                    Name = "Vaclav Havel Airport Prague",
                    City = "Prague",
                    Country = "Czech Republic",
                    Location = new Point(14.2632, 50.1008) { SRID = 4326 }
                },
                new Airport
                {
                    Code = "WAW",
                    Name = "Warsaw Chopin Airport",
                    City = "Warsaw",
                    Country = "Poland",
                    Location = new Point(20.9671, 52.1657) { SRID = 4326 }
                },
                new Airport
                {
                    Code = "BUD",
                    Name = "Budapest Ferenc Liszt International Airport",
                    City = "Budapest",
                    Country = "Hungary",
                    Location = new Point(19.2556, 47.4369) { SRID = 4326 }
                }
            };

      await context.Airports.AddRangeAsync(airports);
      await context.SaveChangesAsync();
    }

    private static async Task SeedFlightsAsync(LuftreiseDbContext context)
    {
      var airports = await context.Airports.ToListAsync();

      var kbp = airports.First(a => a.Code == "KBP");
      var lwo = airports.First(a => a.Code == "LWO");
      var prg = airports.First(a => a.Code == "PRG");
      var waw = airports.First(a => a.Code == "WAW");
      var bud = airports.First(a => a.Code == "BUD");

      var today = DateTime.Today;
      var existingFlights = await context.Flights.ToDictionaryAsync(f => f.FlightNumber);

      var flights = new List<Flight>
      {
        new()
        {
          FlightNumber = "LR1001",
          AirlineName = "Luftreise Airways",
          DepartureAirportId = kbp.Id,
          ArrivalAirportId = prg.Id,
          DepartureTime = today.AddDays(1).AddHours(9),
          ArrivalTime = today.AddDays(1).AddHours(11),
          Price = 120m,
          AvailableSeats = 25,
          TotalSeats = 30,
          Status = FlightStatus.Scheduled
        },
        new()
        {
          FlightNumber = "LR1002",
          AirlineName = "Luftreise Airways",
          DepartureAirportId = lwo.Id,
          ArrivalAirportId = waw.Id,
          DepartureTime = today.AddDays(1).AddHours(13),
          ArrivalTime = today.AddDays(1).AddHours(14).AddMinutes(20),
          Price = 90m,
          AvailableSeats = 18,
          TotalSeats = 24,
          Status = FlightStatus.Scheduled
        },
        new()
        {
          FlightNumber = "LR1003",
          AirlineName = "Luftreise Airways",
          DepartureAirportId = kbp.Id,
          ArrivalAirportId = bud.Id,
          DepartureTime = today.AddDays(2).AddHours(8),
          ArrivalTime = today.AddDays(2).AddHours(10),
          Price = 110m,
          AvailableSeats = 20,
          TotalSeats = 28,
          Status = FlightStatus.Scheduled
        },
        new()
        {
          FlightNumber = "LR1004",
          AirlineName = "Luftreise Airways",
          DepartureAirportId = prg.Id,
          ArrivalAirportId = lwo.Id,
          DepartureTime = today.AddDays(2).AddHours(15),
          ArrivalTime = today.AddDays(2).AddHours(17),
          Price = 105m,
          AvailableSeats = 16,
          TotalSeats = 22,
          Status = FlightStatus.Scheduled
        },
        new()
        {
          FlightNumber = "LR1005",
          AirlineName = "Luftreise Airways",
          DepartureAirportId = waw.Id,
          ArrivalAirportId = kbp.Id,
          DepartureTime = today.AddDays(3).AddHours(10),
          ArrivalTime = today.AddDays(3).AddHours(12),
          Price = 115m,
          AvailableSeats = 22,
          TotalSeats = 30,
          Status = FlightStatus.Scheduled
        },
        new()
        {
          FlightNumber = "LR1006",
          AirlineName = "Luftreise Airways",
          DepartureAirportId = bud.Id,
          ArrivalAirportId = kbp.Id,
          DepartureTime = today.AddDays(3).AddHours(18),
          ArrivalTime = today.AddDays(3).AddHours(20),
          Price = 130m,
          AvailableSeats = 14,
          TotalSeats = 20,
          Status = FlightStatus.Scheduled
        }
      };

      foreach (var flight in flights)
      {
        if (existingFlights.TryGetValue(flight.FlightNumber, out var existingFlight))
        {
          existingFlight.AirlineName = flight.AirlineName;
          existingFlight.DepartureAirportId = flight.DepartureAirportId;
          existingFlight.ArrivalAirportId = flight.ArrivalAirportId;
          existingFlight.DepartureTime = flight.DepartureTime;
          existingFlight.ArrivalTime = flight.ArrivalTime;
          existingFlight.Price = flight.Price;
          existingFlight.AvailableSeats = flight.AvailableSeats;
          existingFlight.TotalSeats = flight.TotalSeats;
          existingFlight.Status = flight.Status;
        }
        else
        {
          await context.Flights.AddAsync(flight);
        }
      }

      await context.SaveChangesAsync();
    }

    private static async Task SeedAdminAsync(LuftreiseDbContext context)
    {
      const string adminEmail = "admin@luftreise.com";

      var existingAdmin = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
      if (existingAdmin != null)
        return;

      var admin = new User
      {
        Email = adminEmail,
        PasswordHash = HashPassword("Admin123!"),
        FirstName = "Admin",
        LastName = "Luftreise",
        PhoneNumber = "+380000000000",
        City = "Uzhhorod",
        Country = "Ukraine",
        BirthDate = new DateTime(2000, 1, 1),
        AvatarPath = null,
        CreatedAt = DateTime.UtcNow,
        Role = UserRole.Admin
      };

      await context.Users.AddAsync(admin);
      await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
      using var sha256 = SHA256.Create();
      var bytes = Encoding.UTF8.GetBytes(password);
      var hash = sha256.ComputeHash(bytes);
      return Convert.ToBase64String(hash);
    }
  }
}
