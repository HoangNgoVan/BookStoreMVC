using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreMVC.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;

        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index(int pageIndex)
        {
            IQueryable<Product> query = context.Products;

            query = query.OrderByDescending(p => p.Id);

            //pagination functionality
            if(pageIndex <1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = await query.ToListAsync();

            ViewBag.Products = products;
            ViewBag.TotalPages = totalPages;   
            ViewBag.PageIndex = pageIndex;

            return View();
        }
    }
}
