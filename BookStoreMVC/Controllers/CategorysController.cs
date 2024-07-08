using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStoreMVC.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class CategorysController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5;

        public CategorysController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public async Task<IActionResult> Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            try
            {
                //select all category
                IQueryable<Category> query = context.Categorys;

                //search funtionality
                if (search != null)
                {
                    query = query.Where(c => c.CategoryName.Contains(search));
                }

                // sort functionality
                string[] validColumns = { "CategoryId", "CategoryName" };
                string[] validOrderBy = { "desc", "asc" };

                if (!validColumns.Contains(column))
                {
                    column = "CategoryId";
                }

                if (!validOrderBy.Contains(orderBy))
                {
                    orderBy = "desc";
                }

                if (column == "CategoryName")
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(c => c.CategoryName);
                    }
                    else
                    {
                        query = query.OrderByDescending(c => c.CategoryName);
                    }
                }
                else
                {
                    if (orderBy == "asc")
                    {
                        query = query.OrderBy(c => c.CategoryId);
                    }
                    else
                    {
                        query = query.OrderByDescending(c => c.CategoryId);
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

                var categorys = await query.ToListAsync();
                ViewData["PageIndex"] = pageIndex;
                ViewData["TotalPages"] = totalPages;

                ViewData["Search"] = search ?? "";

                ViewData["Column"] = column;
                ViewData["OrderBy"] = orderBy;

                return View(categorys);

            }
            catch (Exception ex)
            {
                TempData["error"] = "Không tải được dữ liệu danh mục";
                LogEvents.LogToFile("Category View Gett All", ex.ToString(), environment);
                return RedirectToAction("Index", "Home");

            }
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category categoryView)
        {
            try
            {
                if (categoryView.CategoryName == null)
                {
                    ModelState.AddModelError("CategoryName", "Chưa có tên danh mục");
                }

                if (!ModelState.IsValid)
                {
                    return View(categoryView);
                }


                bool checkCategoryName = context.Categorys.Any(c => c.CategoryName == categoryView.CategoryName);
                if (checkCategoryName)
                {
                    TempData["error"] = "Danh mục đã tồn tại";
                    return View();
                }

                // save the new product in the database
                Category category = new Category()
                {
                    CategoryName = categoryView.CategoryName == null ? "" : categoryView.CategoryName,
                };

                await context.Categorys.AddAsync(category);
                await context.SaveChangesAsync();
                TempData["success"] = "Tạo thành công danh mục";
                return RedirectToAction("Index", "Categorys");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi tạo danh mục";
                LogEvents.LogToFile("Create Category Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Categorys");
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await context.Categorys.FindAsync(id);

                if (category == null)
                {
                    return RedirectToAction("Index", "Categorys");
                }


                ViewData["CategoryId"] = category.CategoryId;

                return View(category);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không tải được danh mục để sửa";
                LogEvents.LogToFile("Edit Category View", ex.ToString(), environment);
                return RedirectToAction("Index", "Categorys");

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category categoryUpdate)
        {
            try
            {
                var category = await context.Categorys.FindAsync(id);

                if (category == null)
                {
                    return RedirectToAction("Index", "Categorys");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["CategoryId"] = category.CategoryId;

                    return View(categoryUpdate);
                }

                //update the category in the database
                category.CategoryName = categoryUpdate.CategoryName == null ? "" : categoryUpdate.CategoryName;

                await context.SaveChangesAsync();
                TempData["success"] = "Sửa thành công danh mục";

                return RedirectToAction("Index", "Categorys");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không sửa được danh mục";
                LogEvents.LogToFile("Edit Category Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Categorys");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await context.Categorys.FindAsync(id);

                if (category == null)
                {
                    return RedirectToAction("Index", "Categorys");
                }


                context.Categorys.Remove(category);
                await context.SaveChangesAsync(true);
                TempData["success"] = "Xóa thành công danh mục";

                return RedirectToAction("Index", "Categorys");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi không xóa được danh mục";
                LogEvents.LogToFile("Delete Category Post", ex.ToString(), environment);
                return RedirectToAction("Index", "Categorys");
            }

        }


    }
}
