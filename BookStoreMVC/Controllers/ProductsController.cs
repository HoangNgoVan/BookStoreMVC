using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;

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

        public async Task<IActionResult> Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            try
            {
                //select join
                var query = from p in context.Products
                            join c in context.Categorys on p.CategoryId equals c.CategoryId
                            join b in context.Brands on p.BrandId equals b.BrandId into pcb
                            from b in pcb.DefaultIfEmpty()
                            select new { p, c, b };


                // search funtionality
                if (search != null)
                {
                    query = query.Where(p => p.p.BookName.Contains(search) || p.p.Author.Contains(search));
                }

                // sort functionality
                string[] validColumns = { "Id", "BookName", "BrandNane", "Author", "CategoryName", "Price", "CreatedAt", "Stock", "NumberOfPage", "PublishYear" };
                string[] validOrderBy = { "desc", "asc" };

                if (!validColumns.Contains(column))
                {
                    column = "Id";
                }

                if (!validOrderBy.Contains(orderBy))
                {
                    orderBy = "desc";
                }

                if (column == "BookName")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.BookName);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.BookName);
                    }
                }
                else if (column == "BrandNane")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.b.BrandName);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.b.BrandName);
                    }
                }
                else if (column == "Author")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.Author);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.Author);
                    }
                }
                else if (column == "CategoryName")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.c.CategoryName);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.c.CategoryName);
                    }
                }
                else if (column == "Price")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.Price);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.Price);
                    }
                }
                else if (column == "CreatedAt")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.CreatedAt);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.CreatedAt);
                    }
                }
                else if (column == "Stock")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.Stock);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.Stock);
                    }
                }
                else if (column == "NumberOfPage")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.NumberOfPage);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.NumberOfPage);
                    }
                }
                else if (column == "PublishYear")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.PublishYear);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.PublishYear);
                    }
                }
                else
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(p => p.p.Id);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.p.Id);
                    }
                }

                //pagination functionality
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }

                decimal count = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(count / pageSize);
                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


                //Create Product View Model
                var data = await query.Select(x => new ProductsVm()
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
                    IsStock = x.p.IsStock,
                    NumberOfPage = x.p.NumberOfPage,
                    PublishYear = x.p.PublishYear,
                }).ToListAsync();

                ViewData["PageIndex"] = pageIndex;
                ViewData["TotalPages"] = totalPages;

                ViewData["Search"] = search ?? "";

                ViewData["Column"] = column;
                ViewData["OrderBy"] = orderBy;

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Không tải được dữ liệu sản phầm";
                LogEvents.LogToFile("Product View Gett All", ex.ToString(), environment);
                return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = context.Categorys
                .Select(c => new SelectListItem
                {
                    Text = c.CategoryName,
                    Value = c.CategoryId.ToString()
                });
            ViewBag.CategoryList = CategoryList;

            IEnumerable<SelectListItem> BrandList = context.Brands
             .Select(b => new SelectListItem
             {
                 Text = b.BrandName,
                 Value = b.BrandId.ToString()
             });
            ViewBag.BrandList = BrandList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            try 
            {
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
                    BrandId = productDto.BrandId,
                    Author = productDto.Author,
                    CategoryId = productDto.CategoryId,
                    Price = productDto.Price,
                    Description = productDto.Description == null ? "" : productDto.Description,
                    ImageFileName = newFileName,
                    CreatedAt = DateTime.Now,
                    Stock = productDto.Stock,
                    IsStock = productDto.Stock == 0 ? false : true,
                    NumberOfPage = productDto.NumberOfPage,
                    PublishYear = productDto.PublishYear,
                };

                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
                TempData["success"] = "Tạo thành công sản phẩm";
                return RedirectToAction("Index", "Products");
            }
            catch(Exception ex)
            {
                TempData["error"] = "Lỗi tạo sản phẩm";
                LogEvents.LogToFile("Create Product Post", ex.ToString(), environment);
                return View();
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            try {
                var product = await context.Products.FindAsync(id);

                if (product == null)
                {
                    return RedirectToAction("Index", "Products");
                }

                var productDto = new ProductDto()
                {
                    BookName = product.BookName,
                    BrandId = product.BrandId,
                    Author = product.Author,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    Stock = product.Stock,
                    Description = product.Description,
                    NumberOfPage = product.NumberOfPage,
                    PublishYear = product.PublishYear,
                };

                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("dd/MM/yyyy");

                IEnumerable<SelectListItem> CategoryList = context.Categorys
                .Select(c => new SelectListItem
                {
                    Text = c.CategoryName,
                    Value = c.CategoryId.ToString()
                });
                ViewBag.CategoryList = CategoryList;

                IEnumerable<SelectListItem> BrandList = context.Brands
                 .Select(b => new SelectListItem
                 {
                     Text = b.BrandName,
                     Value = b.BrandId.ToString()
                 });
                ViewBag.BrandList = BrandList;

                return View(productDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không tải được sản phẩm để sửa";
                LogEvents.LogToFile("Edit Product View", ex.ToString(), environment);
                return View();

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            try 
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
                product.BrandId = productDto.BrandId;
                product.Author = productDto.Author;
                product.CategoryId = productDto.CategoryId;
                product.Price = productDto.Price;
                product.Description = productDto.Description == null ? "" : productDto.Description;
                product.Stock = productDto.Stock;
                product.ImageFileName = newFileName;
                product.PublishYear = productDto.PublishYear;
                product.NumberOfPage = productDto.NumberOfPage;

                await context.SaveChangesAsync();

                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex) 
            {
                TempData["error"] = "Lỗi không sửa được sản phẩm";
                LogEvents.LogToFile("Edit Product Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Products");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
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
            catch(Exception ex) 
            {
                TempData["error"] = "Lỗi không xóa được sản phẩm";
                LogEvents.LogToFile("Delete Product Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Products");
            }

        }
    }
}
