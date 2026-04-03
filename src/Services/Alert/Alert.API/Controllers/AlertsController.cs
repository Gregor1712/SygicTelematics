using Alert.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alert.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly AlertDbContext _context;

    public AlertsController(AlertDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _context.Alerts.ToListAsync();
        return Ok(alerts);
    }

    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var alerts = await _context.Alerts
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return Ok(alerts);
    }
}