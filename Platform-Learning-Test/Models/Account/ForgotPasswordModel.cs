using System.ComponentModel.DataAnnotations;

namespace Platform_Learning_Test.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [StringLength(256, ErrorMessage = "Email не должен превышать 256 символов")]
        public string Email { get; set; } 
    }
}