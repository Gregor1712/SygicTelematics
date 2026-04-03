using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Vehicle.Application.Entities;
using Vehicle.Application.Interfaces;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Services;

public class CsvDataSeeder : ICsvDataSeeder
{
    private readonly VehicleDbContext _context;

    public CsvDataSeeder(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (_context.Vehicles.Any()) return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader("Csv/vehicles.csv");
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<VehicleEntity>().ToList();

        _context.Vehicles.AddRange(records);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {records.Count} vehicles.");
    }
}