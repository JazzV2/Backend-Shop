using backend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Core.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.Username);

            modelBuilder.Entity<Order>()
                .HasOne(order => order.Product)
                .WithMany(product => product.Orders)
                .HasForeignKey(order => order.UrlProduct);

            modelBuilder.Entity<Image>()
                .HasOne(image => image.Product)
                .WithMany(product => product.Images)
                .HasForeignKey(image => image.UrlProduct);
        }
    }
}
