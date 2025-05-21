using System.ComponentModel.DataAnnotations;

namespace Platform_Learning_Test.Models.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 256 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Только латинские буквы, цифры, _ и -")]
        public string UserName { get; set; } 

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [StringLength(256, ErrorMessage = "Email не должен превышать 256 символов")]
        public string Email { get; set; } 

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 100 символов")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Пароль должен содержать цифры, заглавные и строчные буквы")]
        public string Password { get; set; } 

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } 
    }
}