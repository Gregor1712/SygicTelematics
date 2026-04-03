using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Location.Application.Entities;
using Location.Application.Interfaces;
using Location.Infrastructure.Data;

namespace Location.Infrastructure.Services;

public class CsvDataSeeder : ICsvDataSeeder
{
    private readonly LocationDbContext _context;

    public CsvDataSeeder(LocationDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (_context.Locations.Any()) return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader("Csv/locations.csv");
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<LocationEntity>().ToList();

        _context.Locations.AddRange(records);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {records.Count} locations.");
    }
}