using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            //select join
            var query = from p in context.Products
                        join c in context.Categorys on p.CategoryId equals c.CategoryId
                        join b in context.Brands on p.BrandId equals b.BrandId into pcb
                        from b in pcb.DefaultIfEmpty()
                        select new { p, c, b };


            // search functionality
            if (search != null && search.Length > 0) 
            {
                query = query.Where(p => p.p.BookName.Contains(search));
            }

            // filter functionality
            if (brand != null && brand.Length > 0) 
            {
                query = query.Where(p => p.b.BrandName.Contains(brand));
            }

            if (category != null && category.Length > 0) 
            {
                query = query.Where(p => p.c.CategoryName.Contains(category));
            }

            // sort functionality
            if(sort == "price_asc")
            {
                query = query.OrderBy(p => p.p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.p.Price);
            }
            else
            {
                // newest products first
                query = query.OrderByDescending(p => p.p.Id);
            }


            //pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            //Create Product View
            var products = await query.Select(x => new ProductsVm()
            {
                Id = x.p.Id,
                BookName = x.p.BookName,
                BrandName = x.b.BrandName,
                Author = x.p.Author,
                CategoryName = x.c.CategoryName,
                Price = x.p.Price,
                Description = x.p.Description,
                ImageFileName = x.p.ImageFileName,
                CreatedAt = x.p.CreatedAt,
                Stock = x.p.Stock,
                NumberOfPage = x.p.NumberOfPage,
                PublishYear = x.p.PublishYear,
            }).ToListAsync();

            ViewBag.Products = products;

            ViewBag.TotalPages = totalPages;
            ViewBag.PageIndex = pageIndex;

            IEnumerable<SelectListItem> CategoryList = context.Categorys.
                Select(c => new SelectListItem
                {
                    Text = c.CategoryName,
                    Value = c.CategoryName
                });
            ViewBag.CategoryList = CategoryList;

            IEnumerable<SelectListItem> BrandList = context.Brands
             .Select(b => new SelectListItem
             {
                 Text = b.BrandName,
                 Value = b.BrandName
             });
            ViewBag.BrandList = BrandList;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort
            };


            return View(storeSearchModel);
        }

        public  async Task<IActionResult> Details(int id)
        {
            //select join
            var productById = await (from p in context.Products
                        join c in context.Categorys on p.CategoryId equals c.CategoryId
                        join b in context.Brands on p.BrandId equals b.BrandId into pcb
                        from b in pcb.DefaultIfEmpty()
                        where p.Id == id
                        select new ProductsVm
                        {
                            Id = p.Id,
                            BookName = p.BookName,
                            BrandName = b.BrandName,
                            Author = p.Author,
                            CategoryName = c.CategoryName,
                            Price = p.Price,
                            Description = p.Description,
                            ImageFileName = p.ImageFileName,
                            CreatedAt = p.CreatedAt,
                            Stock = p.Stock,
                            NumberOfPage = p.NumberOfPage,
                            PublishYear = p.PublishYear,
                        }).FirstOrDefaultAsync();

            return View(productById);
        }
    }
}
