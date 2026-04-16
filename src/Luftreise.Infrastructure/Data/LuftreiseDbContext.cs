using Luftreise.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Luftreise.Infrastructure.Data;

public class LuftreiseDbContext : DbContext
{
    public LuftreiseDbContext(DbContextOptions<LuftreiseDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Passenger> Passengers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).HasColumnType("geography");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AirlineName).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.DepartureAirport)
                .WithMany(a => a.DepartureFlights)
                .HasForeignKey(e => e.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ArrivalAirport)
                .WithMany(a => a.ArrivalFlights)
                .HasForeignKey(e => e.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(e => e.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PassportNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nationality).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Passengers)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
