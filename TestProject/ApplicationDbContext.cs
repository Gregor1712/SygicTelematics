using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestProject.Dbo;
using TestProject.Entities;

public class ApplicationDbContext(DbContextOptions options): IdentityDbContext<AppUser>(options)
{
    //private const string ConnectionString = "Server=localhost;Database=TestDB; TrustServerCertificate=true; Integrated Security=False; Trusted_Connection=True";
    
    // public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //     : base(options)
    // {
    // }
    
    public DbSet<ManufacturerDBO> Manufacturers { get; set; }
    public DbSet<CpuDBO> CPU { get; set; }
    
    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConnectionString);*/
    
    
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //
    //     modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
    // }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // modelBuilder.Entity<IdentityRole>()
        //     .HasData(
        //         new IdentityRole { Id = "user-id", Name = "User", NormalizedName = "USER" },
        //         new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN" }
        //     );
        
        // Configure one-to-many relationship
        /*modelBuilder.Entity<CpuDbo>()
            .HasOne(c => c.Manufacturer)
            .WithMany(m => m.CPUs)
            .HasForeignKey(c => c.ManufacturerDboId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure decimal precision
        modelBuilder.Entity<CpuDbo>()
            .Property(c => c.BaseClock)
            .HasPrecision(4, 2);
        
        modelBuilder.Entity<CpuDbo>()
            .Property(c => c.BoostClock)
            .HasPrecision(4, 2);
        
        modelBuilder.Entity<CpuDbo>()
            .Property(c => c.Price)
            .HasPrecision(10, 2);*/
    }
}

// C:\Users\grego\RiderProjects\TestProject\TestProject> 
// dotnet ef migrations add Initial
// dotnet ef database update

// dotnet ef migrations add AddIdentity