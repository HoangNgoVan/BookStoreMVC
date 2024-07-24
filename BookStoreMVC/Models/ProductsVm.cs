using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class ProductsVm
    {
        public int Id { get; set; }
        public string BookName { get; set; } = "";
        public string BrandName { get; set; } = "";
        public string Author { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int Stock { get; set; }
        public int NumberOfPage { get; set; }
        public int PublishYear { get; set; }

    }
}
