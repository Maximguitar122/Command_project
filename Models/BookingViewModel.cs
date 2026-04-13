using System.ComponentModel.DataAnnotations;
namespace Luftreise_Command_project_.Models
{
    public class BookingViewModel
    {
        public string SeatNumber { get; set; } = string.Empty;
        public int FlightId { get; set; } 
        public string Airline { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public DateTime FlightDate { get; set; }
        public string FlightClass { get; set; } = string.Empty;
        public double Price { get; set; }

        [Required(ErrorMessage = "Введіть ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Некоректний email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть телефон")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер паспорта")]
        public string PassportNumber { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Допустима вага від 0 до 50 кг")]
        public double BaggageWeight { get; set; }
    }
}