using Battery.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Battery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatteryController : ControllerBase
{
    private readonly BatteryDbContext _context;

    public BatteryController(BatteryDbContext context)
    {
        _context = context;
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
}