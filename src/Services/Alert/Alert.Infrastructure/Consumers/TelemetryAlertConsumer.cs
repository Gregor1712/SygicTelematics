using Alert.Application.Entities;
using Alert.Application.Interfaces;
using Alert.Infrastructure.Data;
using MassTransit;
using Shared.Kernel.Events;

namespace Alert.Infrastructure.Consumers;

public class TelemetryAlertConsumer : IConsumer<TelemetryAlertEvent>
{
    private readonly AlertDbContext _context;
    private readonly IEmailService _emailService;

    public TelemetryAlertConsumer(AlertDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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

        try
        {
            await _emailService.SendAlertEmailAsync(msg.AlertType, msg.Message, msg.VehicleId, msg.Timestamp);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email] Failed to send notification: {ex.Message}");
        }
    }
}
