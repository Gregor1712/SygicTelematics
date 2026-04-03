using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using TestProject.Dbo;
using TestProject.Interfaces;

public class CsvDataSeeder : ICsvDataSeeder 
{
    private readonly ApplicationDbContext _context;
    
    public CsvDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedDataAsync()
    {
        // Check if data already exists
        if (_context.Manufacturers.Any())
        {
            Console.WriteLine("Database already seeded.");
            return;
        }
        
        await SeedManufacturersAsync();
        await SeedCPUsAsync();
        
        Console.WriteLine("Database seeded successfully!");
    }
    
    public async Task SeedManufacturersAsync()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };
        
        using var reader = new StreamReader("Csv/manufacturer.csv");
        using var csv = new CsvReader(reader, config);
        var manufacturersRecords = csv.GetRecords<ManufacturerDBO>().ToList();

        var manufacturers = manufacturersRecords.Select(m => new ManufacturerDBO
        {
            Name = m.Name,
            Description = m.Description
        }).ToList();
        
        _context.Manufacturers.AddRange(manufacturers);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Database.OpenConnectionAsync();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Seeded {manufacturers.Count} manufacturers.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error seeding manufacturers: {ex.Message}");
            throw;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }
    
    public async Task SeedCPUsAsync()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
        };
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        using var reader = new StreamReader("Csv/cpu.csv", Encoding.GetEncoding(1250));
        using var csv = new CsvReader(reader, config);
        var csvRecords = csv.GetRecords<CpuDBO>().ToList();
        
        var cpus = csvRecords.Select(c => new CpuDBO
        {
            Name = c.Name,
            Cores = c.Cores,
            Threads = c.Threads,
            BaseClock = c.BaseClock,
            BoostClock = c.BoostClock,
            TDP = c.TDP,
            Socket = c.Socket,
            ReleaseYear = c.ReleaseYear,
            Price = c.Price,
            Description = c.Description,
            ManufacturerDboId = c.ManufacturerDboId
        }).ToList();
        
        _context.CPU.AddRange(cpus);
        
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Database.OpenConnectionAsync();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Seeded {cpus.Count} cpu.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error seeding cpu: {ex.Message}");
            throw;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }
}