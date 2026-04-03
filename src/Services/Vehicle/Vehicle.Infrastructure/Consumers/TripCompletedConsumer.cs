using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Events;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Consumers;

public class TripCompletedConsumer : IConsumer<TripCompletedEvent>
{
    private readonly VehicleDbContext _context;

    public TripCompletedConsumer(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<TripCompletedEvent> context)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == context.Message.VehicleId);

        if (vehicle is null) return;

        vehicle.LastTripId = context.Message.TripId;
        await _context.SaveChangesAsync();

        Console.WriteLine($"[Vehicle] Updated LastTripId for {vehicle.Id}");
    }
}