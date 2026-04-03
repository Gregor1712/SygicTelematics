using Battery.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Battery.Infrastructure.Data;

public class BatteryDbContext(DbContextOptions<BatteryDbContext> options) : DbContext(options)
{
    public DbSet<BatteryStatusEntity> BatteryStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BatteryStatusEntity>()
            .HasIndex(x => x.VehicleId);

        modelBuilder.Entity<BatteryStatusEntity>()
            .HasIndex(x => x.Timestamp);
    }
}