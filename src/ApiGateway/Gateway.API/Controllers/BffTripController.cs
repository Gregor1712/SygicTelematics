using Gateway.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Trip;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/trips")]
public class BffTripController(TripGrpc.TripGrpcClient client) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var reply = await client.GetByVehicleAsync(new VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return this.ProtoJson(reply);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
    {
        var reply = await client.CreateAsync(request);
        return this.ProtoJsonCreated(reply);
    }
}
