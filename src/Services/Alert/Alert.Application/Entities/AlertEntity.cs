namespace Alert.Application.Entities;

public class AlertEntity
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }

    public string Type { get; set; } = null!;
    public string Message { get; set; } = null!;

    public bool IsResolved { get; set; }

    public DateTime CreatedAt { get; set; }
}