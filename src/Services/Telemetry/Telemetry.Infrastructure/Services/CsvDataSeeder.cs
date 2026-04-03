using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Telemetry.Application.Entities;
using Telemetry.Application.Interfaces;
using Telemetry.Infrastructure.Data;

namespace Telemetry.Infrastructure.Services;

public class CsvDataSeeder : ICsvDataSeeder
{
    private readonly TelemetryDbContext _context;

    public CsvDataSeeder(TelemetryDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        if (_context.TelemetryRecords.Any()) return;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using var reader = new StreamReader("Csv/telemetry_records.csv");
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<TelemetryRecordEntity>().ToList();

        _context.TelemetryRecords.AddRange(records);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {records.Count} telemetry records.");
    }
}