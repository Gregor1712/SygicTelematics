namespace Shared.Kernel.Events;

public record LocationUpdatedEvent
{
    public Guid VehicleId { get; init; }
    public Guid LocationId { get; init; }
    public DateTime Timestamp { get; init; }
}