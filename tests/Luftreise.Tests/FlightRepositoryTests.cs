using Luftreise.Domain.Entities;
using Luftreise.Infrastructure.Data;

namespace Luftreise.Tests;

[TestFixture]
public class FlightRepositoryTests
{
    private LuftreiseDbContext _context = null!;

    [SetUp]
    public void Setup()
    {
        // TODO: Setup in-memory database for testing
    }

    [Test]
    public void Test_FlightEntity_Creation()
    {
        var flight = new Flight
        {
            FlightNumber = "LH123",
            Price = 150.00m,
            AvailableSeats = 100,
            TotalSeats = 150,
            AirlineName = "Lufthansa",
            Status = FlightStatus.Scheduled
        };

        Assert.That(flight.FlightNumber, Is.EqualTo("LH123"));
        Assert.That(flight.Price, Is.EqualTo(150.00m));
        Assert.That(flight.AvailableSeats, Is.EqualTo(100));
    }

    [Test]
    public void Test_Booking_Creation()
    {
        var booking = new Booking
        {
            BookingReference = "LR20260411ABC123",
            NumberOfPassengers = 2,
            TotalPrice = 300.00m,
            Status = BookingStatus.Pending
        };

        Assert.That(booking.BookingReference, Is.Not.Empty);
        Assert.That(booking.NumberOfPassengers, Is.EqualTo(2));
        Assert.That(booking.Status, Is.EqualTo(BookingStatus.Pending));
    }
}
