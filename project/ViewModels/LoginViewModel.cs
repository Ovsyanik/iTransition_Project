using System.ComponentModel.DataAnnotations;

namespace project.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailAddress")]
        [Display(Name = "Email", Prompt = "EmailPlaceholder")]
        public string Email { get; set; }


        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password, ErrorMessage = "PasswordType")]
        [Display(Name = "Password", Prompt = "PasswordPlaceholder")]
        public string Password { get; set; }


        public string ReturnUrl { get; set; }
    }
}
