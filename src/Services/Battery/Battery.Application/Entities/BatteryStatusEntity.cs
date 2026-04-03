namespace Battery.Application.Entities;

public class BatteryStatusEntity
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }

    public int Percentage { get; set; }
    public double Voltage { get; set; }
    public double? Temperature { get; set; }

    public bool IsCharging { get; set; }

    public DateTime Timestamp { get; set; }
}