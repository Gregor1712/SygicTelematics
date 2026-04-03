using Battery.Application.Entities;
using Battery.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Events;

namespace Battery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatteryController : ControllerBase
{
    private readonly BatteryDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public BatteryController(BatteryDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _context.BatteryStatuses.ToListAsync();
        return Ok(statuses);
    }

    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var statuses = await _context.BatteryStatuses
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();
        return Ok(statuses);
    }

    [HttpPost]
    public async Task<IActionResult> Create(BatteryStatusEntity batteryStatus)
    {
        batteryStatus.Id = Guid.NewGuid();
        batteryStatus.Timestamp = DateTime.UtcNow;

        _context.BatteryStatuses.Add(batteryStatus);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new BatteryStatusUpdatedEvent
        {
            VehicleId = batteryStatus.VehicleId,
            BatteryStatusId = batteryStatus.Id,
            Timestamp = batteryStatus.Timestamp
        });

        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId = batteryStatus.VehicleId }, batteryStatus);
    }
}