using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle.Infrastructure.Data;

namespace Vehicle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleDbContext _context;

    public VehiclesController(VehicleDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicles = await _context.Vehicles.ToListAsync();
        return Ok(vehicles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null) return NotFound();
        return Ok(vehicle);
    }
}