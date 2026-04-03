namespace Vehicle.Application.Entities;

public class VehicleEntity
{
    public Guid Id { get; set; }
    public string Vin { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public int Year { get; set; }

    public Guid? CurrentLocationId { get; set; }
    public Guid? BatteryStatusId { get; set; }
    public Guid? LastTripId { get; set; }
}