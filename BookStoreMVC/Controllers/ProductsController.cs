using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 3;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = context.Products;

            // search funtionality
            if(search != null)
            {
                query = query.Where(p => p.BookName.Contains(search) || p.Author.Contains(search));
            }

            // sort functionality
            string[] validColumns = { "Id", "BookName", "Brand", "Author", "Category", "Price", "CreatedAt", "Stock" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if(column == "BookName")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.BookName);
                }
                else
                {
                    query = query.OrderByDescending(p => p.BookName);
                }
            }
            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Author")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Author);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Author);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else if (column == "Stock")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Stock);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Stock);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }
            //query = query.OrderByDescending(p => p.Id);

            //pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

            return View(products);
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            // try catch lỗi
            if (productDto.ImageFile == null) 
            {
                ModelState.AddModelError("ImageFile", "Chưa có hình ảnh của sách");
            }

            if (!ModelState.IsValid) 
            {
                return View(productDto);
            }


            bool checkBookName = context.Products.Any(p => p.BookName == productDto.BookName);
            if (checkBookName) 
            {
                TempData["error"] = "Sản phần đã tồn tại";
                return View();
            }
            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var strean = System.IO.File.Create(imageFullPath)) 
            {
                productDto.ImageFile.CopyTo(strean);
            }

            // save the new product in the database
            Product product = new Product()
            {
                BookName = productDto.BookName,
                Brand = productDto.Brand,
                Author = productDto.Author,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description == null ? "" : productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
                Stock = productDto.Stock,
                IsStock = productDto.Stock == 0 ? false : true,
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            TempData["success"] = "Tạo thành công sản phẩm";
            return RedirectToAction("Index", "Products");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDto()
            {
                BookName= product.BookName,
                Brand = product.Brand,
                Author = product.Author,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
                Stock= product.Stock,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("dd/MM/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("dd/MM/yyyy");

                return View(productDto);
            }

            //Update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var strean = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(strean);
                }

                //delete the old image
                string oldimageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldimageFullPath);

            }

            //update the product in the database
            product.BookName = productDto.BookName;
            product.Brand = productDto.Brand;
            product.Author = productDto.Author;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description == null ? "" : productDto.Description;
            product.Stock = productDto.Stock;
            product.ImageFileName = newFileName;


            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Products");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
             
            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            await context.SaveChangesAsync(true);

            return RedirectToAction("Index", "Products");

        }
    }
}
