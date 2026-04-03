using Microsoft.EntityFrameworkCore;
using Telemetry.Application.Entities;

namespace Telemetry.Infrastructure.Data;

public class TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : DbContext(options)
{
    public DbSet<TelemetryRecordEntity> TelemetryRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TelemetryRecordEntity>()
            .HasIndex(x => x.VehicleId);

        modelBuilder.Entity<TelemetryRecordEntity>()
            .HasIndex(x => x.Timestamp);
    }
}