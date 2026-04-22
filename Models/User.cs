using Luftreise_Command_project_.Models.Validation;
using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Luftreise_Command_project_.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть ПІБ")]
        [StringLength(100, ErrorMessage = "ПІБ не може бути довшим за 100 символів")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Неправильний формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]

        [RegularExpression(@"^\+?[0-9\s\-\(\)]{10,20}$", ErrorMessage = "Неправильний формат телефону")]

        

        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть місто")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть країну")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть дату народження")]

        [MinimumAge(16, ErrorMessage = "Вам має бути щонайменше 16 років")]

        [MaximumAge(24, ErrorMessage = "Вам має бути щонайбільше 24 роки")]

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmPassword { get; set; }

        public bool IsAdmin { get; set; } = false;

        public string? AvatarPath { get; set; }

        [Display(Name = "Фото профілю")]
        public IFormFile? AvatarFile { get; set; }
    }
}
