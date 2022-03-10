using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KissSweet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KissSweet.ViewComponents
{
    public class CategoriesList : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoriesList(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _context.Category.ToListAsync();
            return View(items);
        }
    }
}