using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Events;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Consumers;

public class LocationUpdatedConsumer : IConsumer<LocationUpdatedEvent>
{
    private readonly VehicleDbContext _context;

    public LocationUpdatedConsumer(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<LocationUpdatedEvent> context)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == context.Message.VehicleId);

        if (vehicle is null) return;

        vehicle.CurrentLocationId = context.Message.LocationId;
        await _context.SaveChangesAsync();

        Console.WriteLine($"[Vehicle] Updated CurrentLocationId for {vehicle.Id}");
    }
}