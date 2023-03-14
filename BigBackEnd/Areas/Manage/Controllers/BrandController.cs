﻿using BigBackEnd.DataAccessLayer;
using BigBackEnd.Models;
using BigBackEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BigBackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }


        //[AllowAnonymous]
        [Authorize(Roles ="SuperAdmin,Admin")]
        public async Task<IActionResult> Index(int pageIndex =1)
        {
            IQueryable<Brand> query = _context.Brands
                .Include(b=>b.Products)
                .Where(b=>b.isDeleted == false);
            return View(PageNatedList<Brand>.Create(query,pageIndex,3));
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null) return BadRequest();
            Brand brand = await _context.Brands
                .Include(b=>b.Products.Where(p=>p.isDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == Id && b.isDeleted == false);
            if (brand == null) return NotFound();
            return View(brand);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                 
                return View(brand);
            }

            if (await _context.Brands.AnyAsync(b=>b.isDeleted == false && b.Name.ToLower() == brand.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{brand.Name} adda brand movcuddur");
                return View(brand);
            }

            brand.Name = brand.Name.Trim();
            brand.CreatedAt = DateTime.UtcNow.AddHours(4);
            brand.CreatedBy = "System";

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null) return BadRequest();
            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == Id && b.isDeleted == false);
            if (brand == null) return NotFound();
            return View(brand); 
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? Id, Brand brand)
        {
            if (!ModelState.IsValid) return View(brand);
            if (Id == null) return BadRequest();
            if(Id != brand.Id) return BadRequest();
            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == Id && b.isDeleted == false);
            if(dbBrand == null) return NotFound();
            if(await _context.Brands.AnyAsync(b=>b.isDeleted == false && b.Name.ToLower() == brand.Name.Trim().ToLower() && b.Id != Id))
            {
                ModelState.AddModelError("Name", $"Bu Adda {brand.Name} Brand Artiq Movcuddur");
                return View(brand);
            }

            dbBrand.Name = brand.Name.Trim();
            dbBrand.UpdatedAt= DateTime.UtcNow.AddHours(4);
            dbBrand.UpdatedBy = "System";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
        
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? Id)
        {
            //if (Id == null) return BadRequest();
            //Brand brand = await _context.Brands
            //    .Include(b => b.Products.Where(p => p.isDeleted == false))
            //    .FirstOrDefaultAsync(b => b.Id == Id && b.isDeleted == false);
            //if (brand == null) return NotFound();
            //return View(brand);


            if (Id == null) return BadRequest();
            Brand brand = await _context.Brands
                .Include(b => b.Products.Where(p => p.isDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == Id && b.isDeleted == false);
            if (brand == null) return NotFound();

            brand.isDeleted = true;
            brand.DeletedBy = "System";
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);
            
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public async Task<IActionResult> DeleteBrand(int? Id)
        //{
        //    if (Id == null) return BadRequest();
        //    Brand brand = await _context.Brands
        //        .Include(b => b.Products.Where(p => p.isDeleted == false))
        //        .FirstOrDefaultAsync(b => b.Id == Id && b.isDeleted == false);
        //    if (brand == null) return NotFound();

        //    brand.isDeleted = true;
        //    brand.DeletedBy = "System";
        //    brand.DeletedAt = DateTime.UtcNow.AddHours(4);

        //    //foreach (var item in brand.Products)
        //    //{
        //    //    item.BrandId = null;
        //    //}

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}
    }
    
}