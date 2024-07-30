using Microsoft.CodeAnalysis.Operations;

namespace BookStoreApp.API.ViewModels.User
{
    public class LoginResponseViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
