using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telemetry.Application.Entities;
using Telemetry.Application.Services;
using Telemetry.Infrastructure.Data;

namespace Telemetry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public TelemetryController(TelemetryDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _context.TelemetryRecords.ToListAsync();
        return Ok(records);
    }

    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var records = await _context.TelemetryRecords
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();
        return Ok(records);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelemetryRequest request)
    {
        var record = new TelemetryRecordEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            Type = request.Type,
            Value = request.Value,
            Timestamp = DateTime.UtcNow
        };

        _context.TelemetryRecords.Add(record);
        await _context.SaveChangesAsync();

        var alertEvent = TelemetryThresholdChecker.Check(record.VehicleId, record.Type, record.Value, record.Timestamp);
        if (alertEvent != null)
        {
            await _publishEndpoint.Publish(alertEvent);
            Console.WriteLine($"[Telemetry] Alert published: {alertEvent.AlertType} for vehicle {alertEvent.VehicleId}");
        }

        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId = record.VehicleId }, record);
    }
}

public class CreateTelemetryRequest
{
    public Guid VehicleId { get; set; }
    public string Type { get; set; } = null!;
    public double Value { get; set; }
}