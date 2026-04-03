using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telemetry.Infrastructure.Data;

namespace Telemetry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryDbContext _context;

    public TelemetryController(TelemetryDbContext context)
    {
        _context = context;
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
}