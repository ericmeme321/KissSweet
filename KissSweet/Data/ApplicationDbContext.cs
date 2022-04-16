using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using KissSweet.Models;

namespace KissSweet.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<KissSweet.Models.Product> Product { get; set; }
        public DbSet<KissSweet.Models.Category> Category { get; set; }

        public DbSet<KissSweet.Models.Comment> Comment { get; set; }

        public DbSet<KissSweet.Models.CartItem> CartItem { get; set; }

        public DbSet<KissSweet.Models.OrderItem> OrderItem { get; set; }

        public DbSet<KissSweet.Models.Order> Order { get; set; }
    }
}
