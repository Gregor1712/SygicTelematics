using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Trip;
using Shared.Kernel.Events;
using Trip.Application.Entities;
using Trip.Infrastructure.Data;

namespace Trip.API.GrpcServices;

public class TripGrpcService(TripDbContext context, IPublishEndpoint publishEndpoint)
    : TripGrpc.TripGrpcBase
{
    public override async Task<TripListReply> GetByVehicle(VehicleIdRequest request, ServerCallContext ctx)
    {
        var vehicleId = Guid.Parse(request.VehicleId);
        var trips = await context.Trips
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync();

        var reply = new TripListReply();
        reply.Trips.AddRange(trips.Select(MapToReply));
        return reply;
    }

    public override async Task<TripReply> Create(CreateTripRequest request, ServerCallContext ctx)
    {
        var entity = new TripEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.Parse(request.VehicleId),
            StartTime = request.StartTime.ToDateTime(),
            EndTime = request.EndTime != null ? request.EndTime.ToDateTime() : null,
            DistanceKm = request.DistanceKm,
            AverageSpeed = request.AverageSpeed,
            FuelConsumed = request.HasFuelConsumed ? request.FuelConsumed : null,
            EnergyConsumed = request.HasEnergyConsumed ? request.EnergyConsumed : null,
            StartLocationId = request.HasStartLocationId ? Guid.Parse(request.StartLocationId) : null,
            EndLocationId = request.HasEndLocationId ? Guid.Parse(request.EndLocationId) : null
        };

        context.Trips.Add(entity);
        await context.SaveChangesAsync();

        if (entity.EndTime.HasValue)
        {
            await publishEndpoint.Publish(new TripCompletedEvent
            {
                VehicleId = entity.VehicleId,
                TripId = entity.Id,
                Timestamp = entity.EndTime.Value
            });
        }

        return MapToReply(entity);
    }

    private static TripReply MapToReply(TripEntity e)
    {
        var reply = new TripReply
        {
            Id = e.Id.ToString(),
            VehicleId = e.VehicleId.ToString(),
            StartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(e.StartTime, DateTimeKind.Utc)),
            DistanceKm = e.DistanceKm,
            AverageSpeed = e.AverageSpeed
        };

        if (e.EndTime.HasValue)
            reply.EndTime = Timestamp.FromDateTime(DateTime.SpecifyKind(e.EndTime.Value, DateTimeKind.Utc));
        if (e.FuelConsumed.HasValue) reply.FuelConsumed = e.FuelConsumed.Value;
        if (e.EnergyConsumed.HasValue) reply.EnergyConsumed = e.EnergyConsumed.Value;
        if (e.StartLocationId.HasValue) reply.StartLocationId = e.StartLocationId.Value.ToString();
        if (e.EndLocationId.HasValue) reply.EndLocationId = e.EndLocationId.Value.ToString();

        return reply;
    }
}
