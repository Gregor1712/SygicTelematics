namespace Gateway.API.GraphQL;

public class VehicleType
{
    public string Id { get; set; } = default!;
    public string Vin { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string Manufacturer { get; set; } = default!;
    public int Year { get; set; }
    public string? CurrentLocationId { get; set; }
    public string? BatteryStatusId { get; set; }
    public string? LastTripId { get; set; }
}

public class LocationType
{
    public string Id { get; set; } = default!;
    public string VehicleId { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }
    public string Timestamp { get; set; } = default!;
}

public class BatteryType
{
    public string Id { get; set; } = default!;
    public string VehicleId { get; set; } = default!;
    public int Percentage { get; set; }
    public double Voltage { get; set; }
    public double? Temperature { get; set; }
    public bool IsCharging { get; set; }
    public string Timestamp { get; set; } = default!;
}

public class TripType
{
    public string Id { get; set; } = default!;
    public string VehicleId { get; set; } = default!;
    public string StartTime { get; set; } = default!;
    public string? EndTime { get; set; }
    public double DistanceKm { get; set; }
    public double AverageSpeed { get; set; }
    public double? FuelConsumed { get; set; }
    public double? EnergyConsumed { get; set; }
    public string? StartLocationId { get; set; }
    public string? EndLocationId { get; set; }
}

public class TelemetryType
{
    public string Id { get; set; } = default!;
    public string VehicleId { get; set; } = default!;
    public string Type { get; set; } = default!;
    public double Value { get; set; }
    public string Timestamp { get; set; } = default!;
}

public class AlertType
{
    public string Id { get; set; } = default!;
    public string VehicleId { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool IsResolved { get; set; }
    public string CreatedAt { get; set; } = default!;
}

public class VehicleDetailType
{
    public VehicleType Vehicle { get; set; } = default!;
    public List<LocationType> Locations { get; set; } = [];
    public List<BatteryType> Battery { get; set; } = [];
    public List<TripType> Trips { get; set; } = [];
    public List<TelemetryType> Telemetry { get; set; } = [];
    public List<AlertType> Alerts { get; set; } = [];
}
