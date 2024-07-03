using BookStoreMVC.Models;
using Microsoft.EntityFrameworkCore;


namespace BookStoreMVC.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Brand> Brands { get; set; }

    }
}
