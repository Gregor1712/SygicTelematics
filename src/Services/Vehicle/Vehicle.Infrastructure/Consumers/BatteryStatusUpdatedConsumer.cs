using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Events;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Consumers;

public class BatteryStatusUpdatedConsumer : IConsumer<BatteryStatusUpdatedEvent>
{
    private readonly VehicleDbContext _context;

    public BatteryStatusUpdatedConsumer(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<BatteryStatusUpdatedEvent> context)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == context.Message.VehicleId);

        if (vehicle is null) return;

        vehicle.BatteryStatusId = context.Message.BatteryStatusId;
        await _context.SaveChangesAsync();

        Console.WriteLine($"[Vehicle] Updated BatteryStatusId for {vehicle.Id}");
    }
}