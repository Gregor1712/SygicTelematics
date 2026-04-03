using Microsoft.EntityFrameworkCore;
using Trip.Application.Entities;

namespace Trip.Infrastructure.Data;

public class TripDbContext(DbContextOptions<TripDbContext> options) : DbContext(options)
{
    public DbSet<TripEntity> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TripEntity>()
            .HasIndex(x => x.VehicleId);
    }
}