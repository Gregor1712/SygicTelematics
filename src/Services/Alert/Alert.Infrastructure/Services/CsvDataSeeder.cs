using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Alert.Application.Entities;
using Alert.Application.Interfaces;
using Alert.Infrastructure.Data;

namespace Alert.Infrastructure.Services;

public class CsvDataSeeder : ICsvDataSeeder
{
    private readonly AlertDbContext _context;

    public CsvDataSeeder(AlertDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (_context.Alerts.Any()) return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader("Csv/alerts.csv");
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<AlertEntity>().ToList();

        _context.Alerts.AddRange(records);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {records.Count} alerts.");
    }
}