using Shared.Grpc.Battery;
using Shared.Grpc.Location;
using Shared.Grpc.Telemetry;
using Shared.Grpc.Trip;

namespace Gateway.API.GraphQL;

public class Mutation
{
    public async Task<LocationType> CreateLocation(
        CreateLocationInput input,
        [Service] LocationGrpc.LocationGrpcClient client)
    {
        var request = new CreateLocationRequest
        {
            VehicleId = input.VehicleId.ToString(),
            Latitude = input.Latitude,
            Longitude = input.Longitude
        };
        if (input.Speed.HasValue) request.Speed = input.Speed.Value;

        var reply = await client.CreateAsync(request);
        return new LocationType
        {
            Id = reply.Id,
            VehicleId = reply.VehicleId,
            Latitude = reply.Latitude,
            Longitude = reply.Longitude,
            Speed = reply.HasSpeed ? reply.Speed : null,
            Timestamp = reply.Timestamp
        };
    }

    public async Task<BatteryType> CreateBattery(
        CreateBatteryInput input,
        [Service] BatteryGrpc.BatteryGrpcClient client)
    {
        var request = new CreateBatteryRequest
        {
            VehicleId = input.VehicleId.ToString(),
            Percentage = input.Percentage,
            Voltage = input.Voltage,
            IsCharging = input.IsCharging
        };
        if (input.Temperature.HasValue) request.Temperature = input.Temperature.Value;

        var reply = await client.CreateAsync(request);
        return new BatteryType
        {
            Id = reply.Id,
            VehicleId = reply.VehicleId,
            Percentage = reply.Percentage,
            Voltage = reply.Voltage,
            Temperature = reply.HasTemperature ? reply.Temperature : null,
            IsCharging = reply.IsCharging,
            Timestamp = reply.Timestamp
        };
    }

    public async Task<TelemetryType> CreateTelemetry(
        CreateTelemetryInput input,
        [Service] TelemetryGrpc.TelemetryGrpcClient client)
    {
        var reply = await client.CreateAsync(new CreateTelemetryRequest
        {
            VehicleId = input.VehicleId.ToString(),
            Type = input.Type,
            Value = input.Value
        });
        return new TelemetryType
        {
            Id = reply.Id,
            VehicleId = reply.VehicleId,
            Type = reply.Type,
            Value = reply.Value,
            Timestamp = reply.Timestamp
        };
    }

    public async Task<TripType> CreateTrip(
        CreateTripInput input,
        [Service] TripGrpc.TripGrpcClient client)
    {
        var request = new CreateTripRequest
        {
            VehicleId = input.VehicleId.ToString(),
            StartTime = input.StartTime,
            EndTime = input.EndTime ?? "",
            DistanceKm = input.DistanceKm,
            AverageSpeed = input.AverageSpeed
        };
        if (input.FuelConsumed.HasValue) request.FuelConsumed = input.FuelConsumed.Value;
        if (input.EnergyConsumed.HasValue) request.EnergyConsumed = input.EnergyConsumed.Value;
        if (input.StartLocationId != null) request.StartLocationId = input.StartLocationId;
        if (input.EndLocationId != null) request.EndLocationId = input.EndLocationId;

        var reply = await client.CreateAsync(request);
        return new TripType
        {
            Id = reply.Id,
            VehicleId = reply.VehicleId,
            StartTime = reply.StartTime,
            EndTime = string.IsNullOrEmpty(reply.EndTime) ? null : reply.EndTime,
            DistanceKm = reply.DistanceKm,
            AverageSpeed = reply.AverageSpeed,
            FuelConsumed = reply.HasFuelConsumed ? reply.FuelConsumed : null,
            EnergyConsumed = reply.HasEnergyConsumed ? reply.EnergyConsumed : null,
            StartLocationId = reply.HasStartLocationId ? reply.StartLocationId : null,
            EndLocationId = reply.HasEndLocationId ? reply.EndLocationId : null
        };
    }
}

public record CreateLocationInput(Guid VehicleId, double Latitude, double Longitude, double? Speed);
public record CreateBatteryInput(Guid VehicleId, int Percentage, double Voltage, bool IsCharging, double? Temperature);
public record CreateTelemetryInput(Guid VehicleId, string Type, double Value);
public record CreateTripInput(
    Guid VehicleId, string StartTime, string? EndTime, double DistanceKm, double AverageSpeed,
    double? FuelConsumed, double? EnergyConsumed, string? StartLocationId, string? EndLocationId);
