using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Battery;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/battery")]
public class BffBatteryController(BatteryGrpc.BatteryGrpcClient client) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var reply = await client.GetByVehicleAsync(new VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return Ok(reply.Statuses);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBatteryRequest request)
    {
        var reply = await client.CreateAsync(request);
        return CreatedAtAction(nameof(GetByVehicle), new { vehicleId = reply.VehicleId }, reply);
    }
}
