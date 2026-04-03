using Alert.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Alert.Infrastructure.Data;

public class AlertDbContext(DbContextOptions<AlertDbContext> options) : DbContext(options)
{
    public DbSet<AlertEntity> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AlertEntity>()
            .HasIndex(x => x.VehicleId);
    }
}