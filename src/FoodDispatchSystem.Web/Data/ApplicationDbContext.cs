using FoodDispatchSystem.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace FoodDispatchSystem.Web.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

    public DbSet<Business> Businesses { get; set; } = null!;

    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany()
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetail>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_OrderDetails_Quantity",
                    "[Quantity] >= 1 AND [Quantity] <= 100"));

        modelBuilder.Entity<OrderStatusHistory>()
     .HasOne(h => h.Order)
     .WithMany(o => o.StatusHistory)
     .HasForeignKey(h => h.OrderId)
     .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderStatusHistory>()
            .Property(h => h.ChangedByUserId)
            .HasMaxLength(450)
            .IsRequired();

        modelBuilder.Entity<OrderStatusHistory>()
            .Property(h => h.ChangedByEmail)
            .HasMaxLength(256)
            .IsRequired();
        modelBuilder.Entity<Order>()
    .Property(o => o.CreatedByUserId)
    .HasMaxLength(450);

        modelBuilder.Entity<Order>()
            .Property(o => o.CreatedByEmail)
            .HasMaxLength(256);
    }


}

