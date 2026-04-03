namespace TestProject.DTO;

public class ManufacturerDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ManufacturerDTO()
    {
        Name = string.Empty;
        Description = string.Empty;
    }
}