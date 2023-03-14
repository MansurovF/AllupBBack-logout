using BigBackEnd.DataAccessLayer;
using BigBackEnd.Extensions;
using BigBackEnd.Helpers;
using BigBackEnd.Models;
using BigBackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BigBackEnd.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex=1)
        {
            IQueryable<Category> query = _context.Categories
                .Include(c=>c.Products.Where(p=>p.isDeleted == false))
                .Where(c => c.isDeleted == false && c.isMain)
                .OrderByDescending(c => c.Id);

            



            return View(PageNatedList<Category>.Create(query,pageIndex,3));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.isDeleted == false))
                .ThenInclude(ch => ch.Products.Where(p => p.isDeleted == false))
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (category == null) return NotFound();


            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false && c.isMain).ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false && c.isMain).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Categories.AnyAsync(c=>c.isDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
                return View(category);
            }

            if (category.isMain)
            {
                if (category.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if ((category.File?.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File-in olcusu 300Kb-i kece bilmez");
                }

                

                category.Image = await category.File.CreateFileAsync(_env,"assets","images");
                category.ParentId = null; 
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId","Parent mutleq secilmelidir!");
                    return View(category);
                }
                if (!await _context.Categories.AnyAsync(c=>c.isDeleted == false && c.Id == category.ParentId && c.isMain))
                {
                    ModelState.AddModelError("ParentId", "Parent duzgun secilmelidir!");
                    return View(category);
                }

                category.Image = null;
            }
            

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            
            Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id== id && c.isDeleted == false);

            if (category == null) return NotFound();

            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false && c.isMain).ToListAsync();


            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.isDeleted == false && c.isMain).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (id == null) return BadRequest();
            if(id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.isDeleted == false);

            if (category == null) return NotFound();

            if (await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != category.Id ))
            {
                ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
                return View(category);
            }

            if(dbCategory.isMain != category.isMain)
            {
                ModelState.AddModelError("isMain", "Category veziyyeti deyise bilmez");
                return View(category);
            }

            if (dbCategory.isMain && category.File != null)
            {
                if (category.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if (category.File.CheckFileLenght(300))
                {
                    ModelState.AddModelError("File", "File-in olcusu 300Kb-i kece bilmez");
                }

                FileHelper.DeleteFile(dbCategory.Image, _env, "assets", "images");

                dbCategory.Image = await category.File.CreateFileAsync(_env, "assets", "images");
                
            }
            else
            {
                if (category.ParentId != dbCategory.ParentId)
                {
                    if (category.ParentId == null)
                    {
                        ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir!");
                        return View(category);
                    }
                    if (!await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == category.ParentId && c.isMain))
                    {
                        ModelState.AddModelError("ParentId", "Parent duzgun secilmelidir!");
                        return View(category);
                    }

                    dbCategory.ParentId = category.ParentId;
                }
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbCategory.UpdatedBy = "System";

            //await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.isDeleted == false))
                .ThenInclude(ch => ch.Products.Where(p => p.isDeleted == false))
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (category == null) return NotFound();


            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.isDeleted == false))
                .ThenInclude(ch => ch.Products.Where(p => p.isDeleted == false))
                .Include(c => c.Products.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(c => c.isDeleted == false && c.Id == id);
            if (category == null) return NotFound();

            if (category.Children != null && category.Children.Count() > 0)
            {
                foreach (Category child in category.Children)
                {
                    child.isDeleted = true;
                    child.DeletedBy = "System";
                    child.DeletedAt = DateTime.UtcNow.AddHours(4);

                    if (child.Products != null && child.Products.Count() > 0)
                    {
                        foreach (Product product in child.Products)
                        {
                            product.CategoryId = null;
                        }
                    }
                }
            }

            if (category.Products != null && category.Products.Count() > 0)
            {
                foreach (Product product in category.Products)
                {
                    product.CategoryId = null;
                }
            }

            if (!string.IsNullOrWhiteSpace(category.Image))
            {
                FileHelper.DeleteFile(category.Image, _env, "assets", "images");

            }


            category.isDeleted = true;
            category.DeletedBy = "System";
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
