using BigBackEnd.DataAccessLayer;
using BigBackEnd.Extensions;
using BigBackEnd.Helpers;
using BigBackEnd.Models;
using BigBackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigBackEnd.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {

            IQueryable<Product> queries = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductTags.Where(pt => pt.isDeleted == false)).ThenInclude(p => p.Tag)
                .Where(p => p.isDeleted == false);


            return View(PageNatedList<Product>.Create(queries, pageIndex, 3));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _context.Brands.Where(c => c.isDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.isDeleted == false).ToListAsync();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Kateqoriya Mutleq secilmelidir!");
                return View(product);
            }
            if (product.BrandId == null)
            {
                ModelState.AddModelError("BrandId", "Brend mutleq secilmelidir!");
                return View(product);
            }
            if (await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun kateqoriya secin!");
                return View(product);
            }
            if (await _context.Brands.AnyAsync(c => c.isDeleted == false && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Duzgun brend secin!");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.MainFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("MainFile", "MainFile mutleqdir!");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                if (product.HoverFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.HoverFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                product.HoverImage = await product.HoverFile.CreateFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("HoverFile", "HoverFile mutleqdir!");
                return View(product);
            }

            //Many to Many
            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.isDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"{tagId} deyeri yalnisdir!");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };
                    productTags.Add(productTag);
                    
                }
                product.ProductTags = productTags;
            }

            if (product.Files != null & product.Files.Count() > 6)
            {
                ModelState.AddModelError("Files","File sayi coxdur! 6 dan cox fayl gondermek olmaz!");
                return View(product);
            }

            
            //MultiFile Upload
            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil novu duzgun deyil");
                        return View(product);
                    }
                    if (file.CheckFileLenght(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil olcusu coxdur!");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Iamge = await file.CreateFileAsync(_env, "assets", "images", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);


                }
                product.ProductImages = productImages;
            }
            else
            {
                ModelState.AddModelError("Files", "Sekil mutleq secilmelidir!");
                return View(product);
            }

            string seria = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0,2);
            seria += _context.Brands.FirstOrDefault(c => c.Id == product.BrandId).Name.Substring(0, 2);
            seria = seria.ToLower();

            int code = _context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                (int)_context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            product.Seria = seria;
            product.Code = code;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.CreatedBy = "System";

            

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products
                .Include(p=>p.ProductTags.Where(pt=>pt.isDeleted == false))
                .Include(p=>p.ProductImages.Where(pi=>pi.isDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.isDeleted == false);

            if (product == null) return NotFound();

            ViewBag.Brands = await _context.Brands.Where(c => c.isDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.isDeleted == false).ToListAsync();

            product.TagIds = product.ProductTags?.Select(x => x.TagId);
            

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Brands = await _context.Brands.Where(c => c.isDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.isDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.isDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (id == null)
            {
                return BadRequest();
            }
            if (id == product.Id) return BadRequest();

            Product dbproduct = await _context.Products
                .Include(p => p.ProductTags.Where(pt => pt.isDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.isDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.isDeleted == false);

            if (dbproduct == null) return NotFound();

            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Kateqoriya Mutleq secilmelidir!");
                return View(product);
            }
            if (product.BrandId == null)
            {
                ModelState.AddModelError("BrandId", "Brend mutleq secilmelidir!");
                return View(product);
            }
            if (await _context.Categories.AnyAsync(c => c.isDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun kateqoriya secin!");
                return View(product);
            }
            if (await _context.Brands.AnyAsync(c => c.isDeleted == false && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Duzgun brend secin!");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.MainFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.MainImage, _env, "assets", "images", "product");


                dbproduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "images", "product");
            }

            if (product.HoverFile != null)
            {
                if (product.HoverFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.HoverFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.HoverImage, _env, "assets", "images", "product");


                dbproduct.HoverImage = await product.HoverFile.CreateFileAsync(_env, "assets", "images", "product");
            }

            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                _context.ProductTags.RemoveRange(dbproduct.ProductTags);

                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.isDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"{tagId} deyeri yalnisdir!");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };
                    productTags.Add(productTag);

                }
                dbproduct.ProductTags.AddRange(productTags);
            }

            int canUpload = 6 - (dbproduct.ProductImages != null ? dbproduct.ProductImages.Count : 0);

            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} shekil yukleye bilersiz!");
                dbproduct.TagIds = product.TagIds;
                return View(dbproduct);
            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil novu duzgun deyil");
                        return View(product);
                    }
                    if (file.CheckFileLenght(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil olcusu coxdur!");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Iamge = await file.CreateFileAsync(_env, "assets", "images", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);


                }
                dbproduct.ProductImages.AddRange(productImages);
            }

            dbproduct.Title = product.Title;
            dbproduct.Description = product.Description;
            dbproduct.Price = product.Price;
            dbproduct.DiscountedPrice = product.DiscountedPrice;
            dbproduct.ExTax = product.ExTax;
            dbproduct.Count = product.Count;
            dbproduct.IsBestSeller = product.IsBestSeller;
            dbproduct.IsFeatured = product.IsFeatured;
            dbproduct.IsNewArrival= product.IsNewArrival;
            dbproduct.UpdatedBy = "System";
            dbproduct.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id,int? imageId)
        {

            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.isDeleted == false))
                .FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == id);
            
            if (product == null) return NotFound();

            if (!product.ProductImages.Any(pi => pi.Id == imageId)) return BadRequest();

            if (product.ProductImages.Count <= 1)
            {
                return BadRequest();
            }


            product.ProductImages.FirstOrDefault(p => p.Id == imageId).isDeleted = true;
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedBy = "System";
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(p => p.Id == imageId).Iamge, _env, "assets", "images", "product");



            return PartialView("_ProductImagePartial",product.ProductImages.Where(pi=>pi.isDeleted == false).ToList());
        }
    }
}
