﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BigBackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class DashboardController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}