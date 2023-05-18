using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class SampleDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();
            _ = optionsBuilder.UseSqlServer("<notused>");

            var dbContext = (SampleDbContext)Activator.CreateInstance(typeof(SampleDbContext), optionsBuilder.Options)!;
            return dbContext;
        }
    }
}
