using System.ComponentModel.DataAnnotations;

namespace Luftreise_Command_project_.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Введіть коректний email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}