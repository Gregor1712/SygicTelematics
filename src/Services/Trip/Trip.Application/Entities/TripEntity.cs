namespace Trip.Application.Entities;

public class TripEntity
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public double DistanceKm { get; set; }
    public double AverageSpeed { get; set; }

    public double? FuelConsumed { get; set; }
    public double? EnergyConsumed { get; set; }

    public Guid? StartLocationId { get; set; }
    public Guid? EndLocationId { get; set; }
}