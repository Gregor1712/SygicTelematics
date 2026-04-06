using Shared.Grpc.Alert;
using Shared.Grpc.Battery;
using Shared.Grpc.Location;
using Shared.Grpc.Telemetry;
using Shared.Grpc.Trip;
using Shared.Grpc.Vehicle;

namespace Gateway.API.GraphQL;

public class Query
{
    public async Task<VehicleDetailType> GetVehicle(
        Guid id,
        [Service] VehicleGrpc.VehicleGrpcClient vehicleClient,
        [Service] LocationGrpc.LocationGrpcClient locationClient,
        [Service] BatteryGrpc.BatteryGrpcClient batteryClient,
        [Service] TripGrpc.TripGrpcClient tripClient,
        [Service] TelemetryGrpc.TelemetryGrpcClient telemetryClient,
        [Service] AlertGrpc.AlertGrpcClient alertClient)
    {
        var vehicleIdStr = id.ToString();

        var vehicleTask = vehicleClient.GetByIdAsync(
            new Shared.Grpc.Vehicle.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var locationTask = locationClient.GetByVehicleAsync(
            new Shared.Grpc.Location.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var batteryTask = batteryClient.GetByVehicleAsync(
            new Shared.Grpc.Battery.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var tripTask = tripClient.GetByVehicleAsync(
            new Shared.Grpc.Trip.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var telemetryTask = telemetryClient.GetByVehicleAsync(
            new Shared.Grpc.Telemetry.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var alertTask = alertClient.GetByVehicleAsync(
            new Shared.Grpc.Alert.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;

        await Task.WhenAll(vehicleTask, locationTask, batteryTask, tripTask, telemetryTask, alertTask);

        var vehicle = await vehicleTask;
        var locations = await locationTask;
        var battery = await batteryTask;
        var trips = await tripTask;
        var telemetry = await telemetryTask;
        var alerts = await alertTask;

        return new VehicleDetailType
        {
            Vehicle = new VehicleType
            {
                Id = vehicle.Id,
                Vin = vehicle.Vin,
                Model = vehicle.Model,
                Manufacturer = vehicle.Manufacturer,
                Year = vehicle.Year,
                CurrentLocationId = vehicle.HasCurrentLocationId ? vehicle.CurrentLocationId : null,
                BatteryStatusId = vehicle.HasBatteryStatusId ? vehicle.BatteryStatusId : null,
                LastTripId = vehicle.HasLastTripId ? vehicle.LastTripId : null
            },
            Locations = locations.Locations.Select(l => new LocationType
            {
                Id = l.Id,
                VehicleId = l.VehicleId,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Speed = l.HasSpeed ? l.Speed : null,
                Timestamp = l.Timestamp
            }).ToList(),
            Battery = battery.Statuses.Select(b => new BatteryType
            {
                Id = b.Id,
                VehicleId = b.VehicleId,
                Percentage = b.Percentage,
                Voltage = b.Voltage,
                Temperature = b.HasTemperature ? b.Temperature : null,
                IsCharging = b.IsCharging,
                Timestamp = b.Timestamp
            }).ToList(),
            Trips = trips.Trips.Select(t => new TripType
            {
                Id = t.Id,
                VehicleId = t.VehicleId,
                StartTime = t.StartTime,
                EndTime = string.IsNullOrEmpty(t.EndTime) ? null : t.EndTime,
                DistanceKm = t.DistanceKm,
                AverageSpeed = t.AverageSpeed,
                FuelConsumed = t.HasFuelConsumed ? t.FuelConsumed : null,
                EnergyConsumed = t.HasEnergyConsumed ? t.EnergyConsumed : null,
                StartLocationId = t.HasStartLocationId ? t.StartLocationId : null,
                EndLocationId = t.HasEndLocationId ? t.EndLocationId : null
            }).ToList(),
            Telemetry = telemetry.Records.Select(r => new TelemetryType
            {
                Id = r.Id,
                VehicleId = r.VehicleId,
                Type = r.Type,
                Value = r.Value,
                Timestamp = r.Timestamp
            }).ToList(),
            Alerts = alerts.Alerts.Select(a => new AlertType
            {
                Id = a.Id,
                VehicleId = a.VehicleId,
                Type = a.Type,
                Message = a.Message,
                IsResolved = a.IsResolved,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<List<LocationType>> GetLocations(
        Guid vehicleId,
        [Service] LocationGrpc.LocationGrpcClient client)
    {
        var reply = await client.GetByVehicleAsync(
            new Shared.Grpc.Location.VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return reply.Locations.Select(l => new LocationType
        {
            Id = l.Id, VehicleId = l.VehicleId, Latitude = l.Latitude,
            Longitude = l.Longitude, Speed = l.HasSpeed ? l.Speed : null, Timestamp = l.Timestamp
        }).ToList();
    }

    public async Task<List<BatteryType>> GetBattery(
        Guid vehicleId,
        [Service] BatteryGrpc.BatteryGrpcClient client)
    {
        var reply = await client.GetByVehicleAsync(
            new Shared.Grpc.Battery.VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return reply.Statuses.Select(b => new BatteryType
        {
            Id = b.Id, VehicleId = b.VehicleId, Percentage = b.Percentage,
            Voltage = b.Voltage, Temperature = b.HasTemperature ? b.Temperature : null,
            IsCharging = b.IsCharging, Timestamp = b.Timestamp
        }).ToList();
    }

    public async Task<List<TripType>> GetTrips(
        Guid vehicleId,
        [Service] TripGrpc.TripGrpcClient client)
    {
        var reply = await client.GetByVehicleAsync(
            new Shared.Grpc.Trip.VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return reply.Trips.Select(t => new TripType
        {
            Id = t.Id, VehicleId = t.VehicleId, StartTime = t.StartTime,
            EndTime = string.IsNullOrEmpty(t.EndTime) ? null : t.EndTime,
            DistanceKm = t.DistanceKm, AverageSpeed = t.AverageSpeed,
            FuelConsumed = t.HasFuelConsumed ? t.FuelConsumed : null,
            EnergyConsumed = t.HasEnergyConsumed ? t.EnergyConsumed : null,
            StartLocationId = t.HasStartLocationId ? t.StartLocationId : null,
            EndLocationId = t.HasEndLocationId ? t.EndLocationId : null
        }).ToList();
    }

    public async Task<List<TelemetryType>> GetTelemetry(
        Guid vehicleId,
        [Service] TelemetryGrpc.TelemetryGrpcClient client)
    {
        var reply = await client.GetByVehicleAsync(
            new Shared.Grpc.Telemetry.VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return reply.Records.Select(r => new TelemetryType
        {
            Id = r.Id, VehicleId = r.VehicleId, Type = r.Type,
            Value = r.Value, Timestamp = r.Timestamp
        }).ToList();
    }

    public async Task<List<AlertType>> GetAlerts(
        Guid vehicleId,
        [Service] AlertGrpc.AlertGrpcClient client)
    {
        var reply = await client.GetByVehicleAsync(
            new Shared.Grpc.Alert.VehicleIdRequest { VehicleId = vehicleId.ToString() });
        return reply.Alerts.Select(a => new AlertType
        {
            Id = a.Id, VehicleId = a.VehicleId, Type = a.Type,
            Message = a.Message, IsResolved = a.IsResolved, CreatedAt = a.CreatedAt
        }).ToList();
    }
}
