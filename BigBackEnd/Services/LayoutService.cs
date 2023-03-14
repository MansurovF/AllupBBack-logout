﻿using BigBackEnd.DataAccessLayer;
using BigBackEnd.Interfaces;
using BigBackEnd.Models;
using BigBackEnd.ViewModels.BasketViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BigBackEnd.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        public LayoutService(AppDbContext context, IHttpContextAccessor httpcontextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
        }

        public async Task<List<BasketVM>> GetBasket()
        {
            string cookie = _httpcontextAccessor.HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p=>p.isDeleted == false && p.Id == basketVM.Id);
                    if (product != null)
                    {
                        basketVM.Title = product.Title;
                        basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        basketVM.Image = product.MainImage;
                        basketVM.ExTax = product.ExTax;
                    }

                }

                return basketVMs;
            }
            return new List<BasketVM>();      
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories
                .Include(c=>c.Children.Where(c=>c.isDeleted == false))
                .Where(c=>c.isDeleted == false && c.isMain).ToListAsync();
        }

        public async Task<IDictionary<string,string>> GetSettings()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);

            return settings;
        }
    }
}
