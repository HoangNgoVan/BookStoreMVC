using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [MaxLength(300)]
        public string CategoryName { get; set; } = "";
        public DateTime CreatedAt { get; set; }

    }
}
