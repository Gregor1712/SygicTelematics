using Catalog.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<ManufacturerDbo> Manufacturers { get; set; }
    public DbSet<CpuDbo> CPU { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}