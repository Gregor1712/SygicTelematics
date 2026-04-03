using CsvHelper.Configuration.Attributes;

namespace TestProject.Dbo;

public class ManufacturerDBO
{
    [Ignore]    
    public int Id { get; set; }
    //public required string Name { get; set; }
    //public required string Description { get; set; } 
    
    public string Name { get; set; }
    public string Description { get; set; } 

    //public ICollection<CpuDbo> CPUs { get; set; }
}