using System.ComponentModel.DataAnnotations;

namespace Platform_Learning_Test.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Логин обязателен")]

        [StringLength(256, ErrorMessage = "Логин не должен превышать 256 символов")]

        public string UserName { get; set; } 

        [Required(ErrorMessage = "Пароль обязателен")]

        [DataType(DataType.Password)]

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}