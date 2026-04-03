using Location.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Infrastructure.Data;

public class LocationDbContext(DbContextOptions<LocationDbContext> options) : DbContext(options)
{
    public DbSet<LocationEntity> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocationEntity>()
            .HasIndex(x => x.VehicleId);

        modelBuilder.Entity<LocationEntity>()
            .HasIndex(x => x.Timestamp);
    }
}