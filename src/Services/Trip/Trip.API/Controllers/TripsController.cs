using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trip.Infrastructure.Data;

namespace Trip.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripDbContext _context;

    public TripsController(TripDbContext context)
    {
        _context = context;
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
}