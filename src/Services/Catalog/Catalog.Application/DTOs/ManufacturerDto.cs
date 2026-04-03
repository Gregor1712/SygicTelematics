namespace Catalog.Application.DTOs;

public class ManufacturerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ManufacturerDto()
    {
        Name = string.Empty;
        Description = string.Empty;
    }
}