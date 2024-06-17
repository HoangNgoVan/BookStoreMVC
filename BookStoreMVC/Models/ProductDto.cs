using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class ProductDto
    {
        [Required, MaxLength(300)]
        public string BookName { get; set; } = "";
        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";
        [Required, MaxLength(500)]
        public string Author { get; set; } = "";
        [Required]
        public string Category { get; set; } = "";
        [Required]
        public decimal Price { get; set; }
        [ValidateNever]
        public string Description { get; set; } = "";
        public IFormFile? ImageFile { get; set; }
        public int Stock { get; set; }
    }
}
