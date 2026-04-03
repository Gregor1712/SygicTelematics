using CsvHelper.Configuration.Attributes;

namespace Catalog.Application.Entities;

public class ManufacturerDbo
{
    [Ignore]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}