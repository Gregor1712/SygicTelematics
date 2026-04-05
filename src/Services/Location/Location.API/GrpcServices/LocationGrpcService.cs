using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Location.Application.Entities;
using Location.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Location;
using Shared.Kernel.Events;

namespace Location.API.GrpcServices;

public class LocationGrpcService(LocationDbContext context, IPublishEndpoint publishEndpoint)
    : LocationGrpc.LocationGrpcBase
{
    public override async Task<LocationListReply> GetByVehicle(VehicleIdRequest request, ServerCallContext ctx)
    {
        var vehicleId = Guid.Parse(request.VehicleId);
        var locations = await context.Locations
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();

        var reply = new LocationListReply();
        reply.Locations.AddRange(locations.Select(MapToReply));
        return reply;
    }

    public override async Task<LocationReply> Create(CreateLocationRequest request, ServerCallContext ctx)
    {
        var entity = new LocationEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.Parse(request.VehicleId),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Speed = request.HasSpeed ? request.Speed : null,
            Timestamp = DateTime.UtcNow
        };

        context.Locations.Add(entity);
        await context.SaveChangesAsync();

        await publishEndpoint.Publish(new LocationUpdatedEvent
        {
            VehicleId = entity.VehicleId,
            LocationId = entity.Id,
            Timestamp = entity.Timestamp
        });

        return MapToReply(entity);
    }

    private static LocationReply MapToReply(LocationEntity e)
    {
        var reply = new LocationReply
        {
            Id = e.Id.ToString(),
            VehicleId = e.VehicleId.ToString(),
            Latitude = e.Latitude,
            Longitude = e.Longitude,
            Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(e.Timestamp, DateTimeKind.Utc))
        };
        if (e.Speed.HasValue) reply.Speed = e.Speed.Value;
        return reply;
    }
}
