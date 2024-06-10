using Microsoft.EntityFrameworkCore;

namespace RozetkaParser
{
    public class AppDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductsData>()
                .HasMany(pd => pd.Data);
        }

        public DbSet<ProductsData> ProductsDatas { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=RozetkaParseDb;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}
