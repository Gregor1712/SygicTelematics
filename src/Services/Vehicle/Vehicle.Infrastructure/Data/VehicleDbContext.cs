using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Entities;

namespace Vehicle.Infrastructure.Data;

public class VehicleDbContext(DbContextOptions<VehicleDbContext> options) : DbContext(options)
{
    public DbSet<VehicleEntity> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VehicleEntity>()
            .HasIndex(x => x.Vin)
            .IsUnique();
    }
}