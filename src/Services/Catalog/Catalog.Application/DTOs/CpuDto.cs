using Shared.Kernel.Entities;

namespace Catalog.Application.DTOs;

public class CpuDto : BaseEntity
{
    public required string Name { get; set; }
    public int Cores { get; set; }
    public int Threads { get; set; }
    public decimal BaseClock { get; set; }
    public decimal BoostClock { get; set; }
    public int TDP { get; set; }
    public required string Socket { get; set; }
    public int ReleaseYear { get; set; }
    public decimal Price { get; set; }
    public required string Description { get; set; }
    public int ManufacturerDboId { get; set; }
    public required ManufacturerDto Manufacturer { get; set; }
}