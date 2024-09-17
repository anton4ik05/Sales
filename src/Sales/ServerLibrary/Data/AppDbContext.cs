using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<Page> Pages { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Image> Images { get; set; }
    
    public DbSet<SystemRole> SystemRoles { get; set; }
    
    public DbSet<UserRole> UserRoles { get; set; }
    
}