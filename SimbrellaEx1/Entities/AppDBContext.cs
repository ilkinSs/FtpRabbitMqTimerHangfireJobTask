using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionService.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //  EntityFrameworkCore\Add-Migration      - add like this couse problem with EntityFramework6??
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
        public DbSet<Date> Dates { get; set; }

        public DbSet<Numeric> Numerics { get; set; }

        public DbSet<Text> Texts { get; set; }

      

    }

}
