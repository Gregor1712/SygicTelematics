namespace Shared.Kernel.Events;

public record TelemetryAlertEvent
{
    public Guid VehicleId { get; init; }
    public string AlertType { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string TelemetryType { get; init; } = null!;
    public double Value { get; init; }
    public DateTime Timestamp { get; init; }
}
