using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class Brand
    {
        public int BrandId { get; set; }
        [MaxLength(300)]
        public string BrandName { get; set; } = "";
    }
}
