using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Battery.Application.Entities;
using Battery.Application.Interfaces;
using Battery.Infrastructure.Data;

namespace Battery.Infrastructure.Services;

public class CsvDataSeeder : ICsvDataSeeder
{
    private readonly BatteryDbContext _context;

    public CsvDataSeeder(BatteryDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (_context.BatteryStatuses.Any()) return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader("Csv/battery_statuses.csv");
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<BatteryStatusEntity>().ToList();

        _context.BatteryStatuses.AddRange(records);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {records.Count} battery statuses.");
    }
}