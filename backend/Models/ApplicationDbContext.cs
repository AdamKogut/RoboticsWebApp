using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<User> Users { get; set; }
  }
}