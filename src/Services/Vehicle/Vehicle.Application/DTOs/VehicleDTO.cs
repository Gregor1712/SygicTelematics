namespace Vehicle.Application.DTOs;

public class VehicleDTO
{
    public Guid Id { get; set; }
    public string Vin { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public int Year { get; set; }
}
