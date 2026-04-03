namespace Telemetry.Application.Entities;

public class TelemetryRecordEntity
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }

    public string Type { get; set; } = null!;
    public double Value { get; set; }

    public DateTime Timestamp { get; set; }
}