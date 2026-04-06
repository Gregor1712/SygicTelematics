using Gateway.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Telemetry;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/telemetry")]
public class BffTelemetryController(TelemetryGrpc.TelemetryGrpcClient client) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var reply = await client.GetByVehicleAsync(new VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return this.ProtoJson(reply);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelemetryRequest request)
    {
        var reply = await client.CreateAsync(request);
        return this.ProtoJsonCreated(reply);
    }
}
