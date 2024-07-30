using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.ViewModels.User
{
    public class RegisterUserViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
