using Alert.Application.Entities;
using Alert.Infrastructure.Data;
using MassTransit;
using Shared.Kernel.Events;

namespace Alert.Infrastructure.Consumers;

public class TelemetryAlertConsumer : IConsumer<TelemetryAlertEvent>
{
    private readonly AlertDbContext _context;

    public TelemetryAlertConsumer(AlertDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<TelemetryAlertEvent> context)
    {
        var msg = context.Message;

        var alert = new AlertEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = msg.VehicleId,
            Type = msg.AlertType,
            Message = msg.Message,
            IsResolved = false,
            CreatedAt = msg.Timestamp
        };

        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        Console.WriteLine($"[Alert] Created {msg.AlertType} alert for vehicle {msg.VehicleId}: {msg.Message}");
    }
}
