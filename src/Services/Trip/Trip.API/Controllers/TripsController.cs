using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Events;
using Trip.Application.Entities;
using Trip.Infrastructure.Data;

namespace Trip.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public TripsController(TripDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var trips = await _context.Trips.ToListAsync();
        return Ok(trips);
    }

    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var trips = await _context.Trips
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync();
        return Ok(trips);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TripEntity trip)
    {
        trip.Id = Guid.NewGuid();

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        if (trip.EndTime.HasValue)
        {
            await _publishEndpoint.Publish(new TripCompletedEvent
            {
                VehicleId = trip.VehicleId,
                TripId = trip.Id,
                Timestamp = trip.EndTime.Value
            });
        }

        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId = trip.VehicleId }, trip);
    }
}