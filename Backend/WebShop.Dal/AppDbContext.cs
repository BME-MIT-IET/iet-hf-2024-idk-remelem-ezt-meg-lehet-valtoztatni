using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using WebShop.Dal.Entities;

namespace WebShop.Dal;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category("Élelmiszer") { Id = 1 },
            new Category("Elektronika") { Id = 2 },
            new Category("Sport") { Id = 3 }
        );
        
        modelBuilder.Entity<Product>().HasData(
            new Product("Alma", 250) { Id = 1, CategoryId = 1, Description = "Az orvost távol tartja" },
            new Product("Banán", 200) { Id = 2, CategoryId = 1 },
            new Product("Tej", 400) { Id = 3, CategoryId = 1, Description = "2.8%-os" },
            new Product("Sajt", 300) { Id = 4, CategoryId = 1 },
            new Product("Alaplap", 50000) { Id = 5, CategoryId = 2 },
            new Product("Processzor", 80000) { Id = 6, CategoryId = 2, Description = "Ryzen 9 9001" },
            new Product("Tápegység", 30000) { Id = 7, CategoryId = 2, Description = "1000W" },
            new Product("UTP kábel", 2000) { Id = 8, CategoryId = 2, Description = "5m" },
            new Product("Labda", 3000) { Id = 9, CategoryId = 3, Description = "Piros, pöttyös"},
            new Product("Kerékpár", 100000) { Id = 10, CategoryId = 3 },
            new Product("Roller", 45000) { Id = 11, CategoryId = 3 }
        );

        modelBuilder.Entity<User>().HasData(
            new User("admin", "admin@admin.com", "ADMIN_admin") { Id = 1, IsAdmin = true},
            new User("user", "user@user.com", "USER_user") { Id = 2, IsAdmin = false}
        );
    }

    public virtual DbSet<Product> Products => Set<Product>();
    public virtual DbSet<Category> Categories => Set<Category>();
    public virtual DbSet<Order> Orders => Set<Order>();
    public virtual DbSet<User> Users => Set<User>();
}