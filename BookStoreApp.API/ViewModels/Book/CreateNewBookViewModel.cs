using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.ViewModels.Book
{
    public class CreateNewBookViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [Range(1000, int.MaxValue)]
        public int? Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string Isbn { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string Summary { get; set; }

        [Required]
        public string? Image { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AuthorId { get; set; }
    }
}
