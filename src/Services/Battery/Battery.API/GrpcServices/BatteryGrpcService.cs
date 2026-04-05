using Battery.Application.Entities;
using Battery.Infrastructure.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Battery;
using Shared.Kernel.Events;

namespace Battery.API.GrpcServices;

public class BatteryGrpcService(BatteryDbContext context, IPublishEndpoint publishEndpoint)
    : BatteryGrpc.BatteryGrpcBase
{
    public override async Task<BatteryListReply> GetByVehicle(VehicleIdRequest request, ServerCallContext ctx)
    {
        var vehicleId = Guid.Parse(request.VehicleId);
        var statuses = await context.BatteryStatuses
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();

        var reply = new BatteryListReply();
        reply.Statuses.AddRange(statuses.Select(MapToReply));
        return reply;
    }

    public override async Task<BatteryReply> Create(CreateBatteryRequest request, ServerCallContext ctx)
    {
        var entity = new BatteryStatusEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.Parse(request.VehicleId),
            Percentage = request.Percentage,
            Voltage = request.Voltage,
            Temperature = request.HasTemperature ? request.Temperature : null,
            IsCharging = request.IsCharging,
            Timestamp = DateTime.UtcNow
        };

        context.BatteryStatuses.Add(entity);
        await context.SaveChangesAsync();

        await publishEndpoint.Publish(new BatteryStatusUpdatedEvent
        {
            VehicleId = entity.VehicleId,
            BatteryStatusId = entity.Id,
            Timestamp = entity.Timestamp
        });

        return MapToReply(entity);
    }

    private static BatteryReply MapToReply(BatteryStatusEntity e)
    {
        var reply = new BatteryReply
        {
            Id = e.Id.ToString(),
            VehicleId = e.VehicleId.ToString(),
            Percentage = e.Percentage,
            Voltage = e.Voltage,
            IsCharging = e.IsCharging,
            Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(e.Timestamp, DateTimeKind.Utc))
        };
        if (e.Temperature.HasValue) reply.Temperature = e.Temperature.Value;
        return reply;
    }
}
