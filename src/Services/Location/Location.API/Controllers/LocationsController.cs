using Location.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Location.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly LocationDbContext _context;

    public LocationsController(LocationDbContext context)
    {
        _context = context;
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
}