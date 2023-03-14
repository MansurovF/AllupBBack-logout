using BigBackEnd.DataAccessLayer;
using BigBackEnd.Models;
using BigBackEnd.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigBackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(s => s.isDeleted == false).ToListAsync();
            IEnumerable<Category> categories = await _context.Categories.Where(c => c.isDeleted == false && c.isMain).ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Sliders = sliders,
                Categories = categories,
                BestSeller = await _context.Products.Where(c => c.isDeleted == false && c.IsBestSeller).ToListAsync(),
                Featured = await _context.Products.Where(c => c.isDeleted == false && c.IsFeatured).ToListAsync(),
                NewArrival = await _context.Products.Where(c => c.isDeleted == false && c.IsNewArrival).ToListAsync()

            };
                return View(homeVM);
        }
        public IActionResult SetCookie()
        {

            HttpContext.Response.Cookies.Append("FirstCookie", "Content");

            return Content("Cookie elave olundu");
        }
        public IActionResult GetCookie()
        {
            string cookie = HttpContext.Request.Cookies["FirstCookie"];

            return Content(cookie);
        }
    }
}
