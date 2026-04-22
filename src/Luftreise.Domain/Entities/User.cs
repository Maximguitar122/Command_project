using Luftreise.Domain.Enums;

namespace Luftreise.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public string? AvatarPath { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime? BirthDate { get; set; }
}
