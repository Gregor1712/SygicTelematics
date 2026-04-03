namespace Shared.Kernel.Events;

public record TripCompletedEvent
{
    public Guid VehicleId { get; init; }
    public Guid TripId { get; init; }
    public DateTime Timestamp { get; init; }
}