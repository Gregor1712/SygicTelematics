namespace Shared.Kernel.Events;

public record BatteryStatusUpdatedEvent
{
    public Guid VehicleId { get; init; }
    public Guid BatteryStatusId { get; init; }
    public DateTime Timestamp { get; init; }
}