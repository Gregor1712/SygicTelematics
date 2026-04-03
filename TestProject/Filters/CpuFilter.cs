using TestProject.Filters.Conditions;

namespace TestProject.Filters;

public class CpuFilter : FilterBase
{
    public StringCondition? Name { get; set; } = new(nameof(Name));
    
    // private string? _name;
    // public string? Name
    // {
    //     get => _name;
    //     set => _name = value?.ToLower();
    // }
    //
    // private string? _socket;
    // public string? Socket
    // {
    //     get => _socket;
    //     set => _socket = value?.ToLower();
    // }
    //
    // private int? _cores;
    // public int? Cores
    // {
    //     get => _cores;
    //     set => _cores = value;
    // }  
    //
    // public string? Sort { get; set; }
    
    // private string? _search;
    // public string Search
    // {
    //     get => _search ?? "";
    //     set => _search = value.ToLower();
    // }
}