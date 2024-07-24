using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookStoreMVC.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5;

        public BrandsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public async Task<IActionResult> Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            try
            {
                //select all category
                IQueryable<Brand> query = context.Brands;

                //search funtionality
                if (search != null)
                {
                    query = query.Where(b => b.BrandName.Contains(search));
                }

                // sort functionality
                string[] validColumns = { "BrandId", "BrandName", "CreatedAt" };
                string[] validOrderBy = { "desc", "asc" };

                if (!validColumns.Contains(column))
                {
                    column = "BrandId";
                }

                if (!validOrderBy.Contains(orderBy))
                {
                    orderBy = "desc";
                }

                if (column == "BrandName")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(b => b.BrandName);
                    }
                    else
                    {
                        query = query.OrderByDescending(b => b.BrandName);
                    }
                }
                else if (column == "CreatedAt")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(b => b.CreatedAt);
                    }
                    else
                    {
                        query = query.OrderByDescending(b => b.CreatedAt);
                    }
                }
                else
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(b => b.BrandId);
                    }
                    else
                    {
                        query = query.OrderByDescending(b => b.BrandId);
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

                var brands = await query.ToListAsync();
                ViewData["PageIndex"] = pageIndex;
                ViewData["TotalPages"] = totalPages;

                ViewData["Search"] = search ?? "";

                ViewData["Column"] = column;
                ViewData["OrderBy"] = orderBy;

                return View(brands);

            }
            catch (Exception ex)
            {
                TempData["error"] = "Không tải được dữ liệu NXB";
                LogEvents.LogToFile("Brand View Gett All", ex.ToString(), environment);
                return RedirectToAction("Index", "Home");

            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brandView)
        {
            try
            {
                if (brandView.BrandName == null)
                {
                    ModelState.AddModelError("BrandName", "Chưa có tên nhà xuất bản");
                }

                if (!ModelState.IsValid)
                {
                    return View(brandView);
                }


                bool checkBrandName = context.Brands.Any(b => b.BrandName == brandView.BrandName);
                if (checkBrandName)
                {
                    TempData["error"] = "Nhà xuất bản đã tồn tại";
                    return View();
                }

                // save the new product in the database
                Brand brand = new Brand()
                {
                    BrandName = brandView.BrandName == null ? "" : brandView.BrandName,
                    CreatedAt = DateTime.Now,
                };

                await context.Brands.AddAsync(brand);
                await context.SaveChangesAsync();
                TempData["success"] = "Tạo thành công nhà xuất bản";
                return RedirectToAction("Index", "Brands");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi tạo nhà xuất bản";
                LogEvents.LogToFile("Create Brand Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Brands");
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var brand = await context.Brands.FindAsync(id);

                if (brand == null)
                {
                    return RedirectToAction("Index", "Brands");
                }


                ViewData["BrandId"] = brand.BrandId;
                ViewData["CreatedAt"] = brand.CreatedAt.ToString("dd/MM/yyyy");


                return View(brand);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không tải được NXB để sửa";
                LogEvents.LogToFile("Edit Brand View", ex.ToString(), environment);
                return RedirectToAction("Index", "Brands");

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Brand brandUpdate)
        {
            try
            {
                var brand = await context.Brands.FindAsync(id);

                if (brand == null)
                {
                    return RedirectToAction("Index", "Brands");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["BrandId"] = brand.BrandId;
                    ViewData["CreatedAt"] = brand.CreatedAt.ToString("dd/MM/yyyy");

                    return View(brandUpdate);
                }

                //update the category in the database
                brand.BrandName = brandUpdate.BrandName == null ? "" : brandUpdate.BrandName;

                await context.SaveChangesAsync();
                TempData["success"] = "Sửa thành công nhà xuất bản";

                return RedirectToAction("Index", "Brands");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không sửa được nhà xuất bản";
                LogEvents.LogToFile("Edit Brand Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Brands");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var brand = await context.Brands.FindAsync(id);

                if (brand == null)
                {
                    return RedirectToAction("Index", "Brands");
                }


                context.Brands.Remove(brand);
                await context.SaveChangesAsync(true);
                TempData["success"] = "Xóa thành công nhà xuất bản";

                return RedirectToAction("Index", "Brands");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không xóa được nhà xuất bản";
                LogEvents.LogToFile("Delete Brand Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Brands");
            }

        }

    }
}
