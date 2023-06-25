using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.AuthorRelated;
using Domain.BookRelated;
using Domain.ConsentRelated;
using Domain.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

        public DbSet<BookDbo> Books { get; set; } = null!;
        public DbSet<AuthorDbo> Authors { get; set; } = null!;
        public DbSet<ConsentDbo> Consents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.ConfigureWarnings(w =>
            {
                _ = w.Log((Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning, LogLevel.Warning));
            });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            PreSaveChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            PreSaveChanges();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void PreSaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified && e.Entity is IDboBase))
            {
                ((IDboBase)entry.Entity).OnSavingModified();
            }
        }
    }
}
