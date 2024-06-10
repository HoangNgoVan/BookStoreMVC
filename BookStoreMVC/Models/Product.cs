using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string BookName { get; set; } = "";
        [MaxLength(100)]
        public string Brand { get; set; } = "";
        [MaxLength(500)]
        public string Author { get; set; } = "";
        [MaxLength(100)]
        public string Category { get; set; } = "";
        [Precision(16, 3)]
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        [MaxLength(300)]
        public string ImageFileName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int Stock { get; set; }
        public bool IsStock { get; set; }

    }
}
