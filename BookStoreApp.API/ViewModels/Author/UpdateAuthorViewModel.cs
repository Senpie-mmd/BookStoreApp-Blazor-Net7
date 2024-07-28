using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.ViewModels.Author
{
    public class UpdateAuthorViewModel : BaseViewModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(250)]
        public string Bio { get; set; }
    }
}
