namespace Location.Application.Entities;

public class LocationEntity
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }

    public DateTime Timestamp { get; set; }
}