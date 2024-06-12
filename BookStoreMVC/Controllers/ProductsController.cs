using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.ToList();
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

            context.Products.Add(product);
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

     
    }
}
