using Gateway.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Location;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/locations")]
public class BffLocationController(LocationGrpc.LocationGrpcClient client) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var reply = await client.GetByVehicleAsync(new VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return this.ProtoJson(reply);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
    {
        var reply = await client.CreateAsync(request);
        return this.ProtoJsonCreated(reply);
    }
}
