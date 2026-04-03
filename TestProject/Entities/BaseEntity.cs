using CsvHelper.Configuration.Attributes;

namespace TestProject.Entities;

public class BaseEntity
{
    [Ignore]
    public int Id { get; set; }  
}