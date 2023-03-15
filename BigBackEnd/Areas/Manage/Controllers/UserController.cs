using BigBackEnd.Areas.Manage.ViewModels.AccountViewModels;
using BigBackEnd.Models;
using BigBackEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigBackEnd.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageIndex=1)
        {
            //List<AppUser> appUsers = await _userManager.Users.
            //    Where(u => u.UserName != User.Identity.Name)
            //    .Select(x=> new UserVM
            //    {
            //        UserName = x.UserName,
            //        Name = x.UserName,
            //        Email= x.Email,
            //        Id = x.Id,
            //        Surname = x.Surname,
            //        Role = (_userManager.GetRolesAsync(x)[0])
            //    })
            //    ToListAsync();
            //asagdakinin qisa formasi


            List<AppUser> appUsers = await _userManager.Users.Where(u => u.UserName != User.Identity.Name).ToListAsync();
            List<UserVM> userVMs = new List<UserVM>();
            foreach (AppUser item in appUsers)
            {
                string rol = (await _userManager.GetRolesAsync(item))[0];

                UserVM userVM = new UserVM
                {
                    Email = item.Email,
                    UserName = item.UserName,
                    Name = item.Name,
                    Surname=item.SurName,
                    Id = item.Id
                };
                userVMs.Add(userVM);
            }

            return View(PageNatedList<UserVM>.Create(userVMs.AsQueryable(),pageIndex,3));
        }
    }
}
