using Alert.Infrastructure.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Alert;
using AlertEntity = Alert.Application.Entities.AlertEntity;

namespace Alert.API.GrpcServices;

public class AlertGrpcService(AlertDbContext context) : AlertGrpc.AlertGrpcBase
{
    public override async Task<AlertListReply> GetByVehicle(VehicleIdRequest request, ServerCallContext ctx)
    {
        var vehicleId = Guid.Parse(request.VehicleId);
        var alerts = await context.Alerts
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var reply = new AlertListReply();
        reply.Alerts.AddRange(alerts.Select(MapToReply));
        return reply;
    }

    private static AlertReply MapToReply(AlertEntity e) => new()
    {
        Id = e.Id.ToString(),
        VehicleId = e.VehicleId.ToString(),
        Type = e.Type,
        Message = e.Message,
        IsResolved = e.IsResolved,
        CreatedAt = e.CreatedAt.ToString("o")
    };
}
