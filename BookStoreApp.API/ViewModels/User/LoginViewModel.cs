using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.ViewModels.User
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
