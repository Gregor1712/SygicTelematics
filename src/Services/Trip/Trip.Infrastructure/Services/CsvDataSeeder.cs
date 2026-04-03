using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Trip.Application.Entities;
using Trip.Application.Interfaces;
using Trip.Infrastructure.Data;

namespace Trip.Infrastructure.Services;

public class CsvDataSeeder : ICsvDataSeeder
{
    private readonly TripDbContext _context;

    public CsvDataSeeder(TripDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (_context.Trips.Any()) return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader("Csv/trips.csv");
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<TripEntity>().ToList();

        _context.Trips.AddRange(records);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {records.Count} trips.");
    }
}