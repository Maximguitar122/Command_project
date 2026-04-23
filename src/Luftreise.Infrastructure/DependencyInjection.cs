using Luftreise.Application.Interfaces;
using Luftreise.Application.Services;
using Luftreise.Infrastructure.Data;
using Luftreise.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luftreise.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<LuftreiseDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            x => x.UseNetTopologySuite()));

    services.AddScoped<IFlightRepository, FlightRepository>();
    services.AddScoped<IBookingRepository, BookingRepository>();
    services.AddScoped<IUserRepository, UserRepository>();

    services.AddScoped<IFlightService, FlightService>();
    services.AddScoped<IBookingService, BookingService>();

    return services;
  }
}
