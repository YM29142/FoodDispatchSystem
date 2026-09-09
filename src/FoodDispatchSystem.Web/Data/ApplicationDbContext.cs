using FoodDispatchSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
}