using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Telemetry;
using Telemetry.Application.Entities;
using Telemetry.Application.Services;
using Telemetry.Infrastructure.Data;

namespace Telemetry.API.GrpcServices;

public class TelemetryGrpcService(TelemetryDbContext context, IPublishEndpoint publishEndpoint)
    : TelemetryGrpc.TelemetryGrpcBase
{
    public override async Task<TelemetryListReply> GetByVehicle(VehicleIdRequest request, ServerCallContext ctx)
    {
        var vehicleId = Guid.Parse(request.VehicleId);
        var records = await context.TelemetryRecords
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();

        var reply = new TelemetryListReply();
        reply.Records.AddRange(records.Select(MapToReply));
        return reply;
    }

    public override async Task<TelemetryReply> Create(CreateTelemetryRequest request, ServerCallContext ctx)
    {
        var entity = new TelemetryRecordEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.Parse(request.VehicleId),
            Type = request.Type,
            Value = request.Value,
            Timestamp = DateTime.UtcNow
        };

        context.TelemetryRecords.Add(entity);
        await context.SaveChangesAsync();

        var alertEvent = TelemetryThresholdChecker.Check(entity.VehicleId, entity.Type, entity.Value, entity.Timestamp);
        if (alertEvent != null)
        {
            await publishEndpoint.Publish(alertEvent);
        }

        return MapToReply(entity);
    }

    private static TelemetryReply MapToReply(TelemetryRecordEntity e) => new()
    {
        Id = e.Id.ToString(),
        VehicleId = e.VehicleId.ToString(),
        Type = e.Type,
        Value = e.Value,
        Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(e.Timestamp, DateTimeKind.Utc))
    };
}
