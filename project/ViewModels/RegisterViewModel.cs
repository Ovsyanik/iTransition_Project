using System.ComponentModel.DataAnnotations;

namespace project.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "UserNameRequired")]
        [Display(Name = "UserName", Prompt = "UserNamePlaceholder")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailAddress")]
        [Display(Name = "Email", Prompt = "EmailPlaceholder")]
        public string Email { get; set; }


        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password, ErrorMessage = "PasswordType")]
        [Display(Name = "Password", Prompt = "PasswordPlaceholder")]
        public string Password { get; set; }


        [DataType(DataType.Password, ErrorMessage = "PasswordType")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "ConfirmedPassword", Prompt = "ConfirmedPasswordPrompt")]
        public string ConfirmedPassword { get; set; } 
    }
}