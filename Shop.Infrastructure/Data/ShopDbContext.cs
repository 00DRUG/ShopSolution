using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Infrastructure.Data
{
    public class ShopDbContext: DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(128);
                entity.Property(p => p.ImgUrl).IsRequired();
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });
            //Add some samples
            modelBuilder.Entity<Product>().HasData(
                new { Id = 1, Name = "Gaming Mouse", ImgUrl = "http://img.com/mouse.jpg", Price = 50.00m, StockQuantity = 10, Description = "High precision mouse" },
                new { Id = 2, Name = "Mechanical Keyboard", ImgUrl = "http://img.com/kb.jpg", Price = 120.00m, StockQuantity = 5, Description = "Clicky keys" },
                new { Id = 3, Name = "Monitor 24", ImgUrl = "http://img.com/monitor.jpg", Price = 200.00m, StockQuantity = 0, Description = "Full HD" }
                );
        }
    }
}
