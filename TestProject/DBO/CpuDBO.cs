using CsvHelper.Configuration.Attributes;
using TestProject.Entities;
using TestProject.Interfaces;

namespace TestProject.Dbo;

public class CpuDBO: BaseEntity
{
    //[Ignore]
    //public int Id { get; set; }
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
    
    // Foreign key
    public int ManufacturerDboId { get; set; }
    
    // Navigation property
    public ManufacturerDBO Manufacturer { get; set; }
}