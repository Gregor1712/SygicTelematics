using Microsoft.AspNetCore.Mvc;
using Shared.Kernel.Wrappers;
using Vehicle.Application.DTOs;
using Vehicle.Application.Filters;
using Vehicle.Application.Interfaces;
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

    [HttpGet(Name = nameof(GetAll))]
    public async Task<ActionResult<PagedResponse<List<VehicleDTO>>>> GetAll(
        [FromServices] IServerService server,
        [FromQuery] VehicleFilter filter,
        [FromQuery] SortFilter sort,
        [FromQuery] PaginationFilter pagination)
    {
        var data = await server.GetVehicle(filter, sort, pagination);
        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null) return NotFound();
        return Ok(vehicle);
    }
}