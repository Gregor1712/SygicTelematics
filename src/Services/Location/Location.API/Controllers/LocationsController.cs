using Location.Application.Entities;
using Location.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Events;

namespace Location.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly LocationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public LocationsController(LocationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locations = await _context.Locations.ToListAsync();
        return Ok(locations);
    }

    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var locations = await _context.Locations
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();
        return Ok(locations);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LocationEntity location)
    {
        location.Id = Guid.NewGuid();
        location.Timestamp = DateTime.UtcNow;

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new LocationUpdatedEvent
        {
            VehicleId = location.VehicleId,
            LocationId = location.Id,
            Timestamp = location.Timestamp
        });

        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId = location.VehicleId }, location);
    }
}