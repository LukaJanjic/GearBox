namespace Infrastructure.Data;

using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class GearBoxContext(DbContextOptions<GearBoxContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GearBoxContext).Assembly);
    }
}
