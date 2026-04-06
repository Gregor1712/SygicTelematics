using Gateway.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Alert;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/alerts")]
public class BffAlertController(AlertGrpc.AlertGrpcClient client) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var reply = await client.GetByVehicleAsync(new VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return this.ProtoJson(reply);
    }
}
