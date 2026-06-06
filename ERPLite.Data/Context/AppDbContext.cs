using ERPLite.Data.Entities.AI;
using ERPLite.Data.Entities.HR;
using ERPLite.Data.Entities.Identity;
using ERPLite.Data.Entities.Inventory;
using ERPLite.Data.Entities.Sales;
using ERPLite.Data.Entities.System;
using ERPLite.Data.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // HR
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    // Inventory
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<StockMovement> stockMovements { get; set; }

    // Sales
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    // AI
    public DbSet<AIReport> AIReports { get; set; }
    public DbSet<AILog> AILogs { get; set; }

    // System
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        ModelBuilderExtensions.ApplySoftDeleteQueryFilters(modelBuilder);
    }


}