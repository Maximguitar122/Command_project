using System.ComponentModel.DataAnnotations;

namespace Luftreise_Command_project_.Models
{
    public class PassengerFormViewModel
    {
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

        [Required(ErrorMessage = "Оберіть місце")]
        public string SeatNumber { get; set; } = "A12";
    }
}
