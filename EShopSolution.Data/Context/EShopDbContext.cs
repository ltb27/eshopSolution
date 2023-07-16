using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopSolution.Data.Context
{
    public class EShopDbContext : DbContext
    {
        public EShopDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
 