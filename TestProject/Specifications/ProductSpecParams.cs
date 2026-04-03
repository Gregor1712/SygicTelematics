namespace TestProject.Specifications;

public class ProductSpecParams : PagingParams
{
    private string? _name;
    public string? Name
    {
        get => _name;
        set => _name = value?.ToLower();
    }
    
    private string? _socket;
    public string? Socket
    {
        get => _socket;
        set => _socket = value?.ToLower();
    }
    
    private int? _cores;
    public int? Cores
    {
        get => _cores;
        set => _cores = value;
    }
    
    
    // private List<string> _name = [];
    // public List<string> Name
    // {
    //     get => _name;
    //     set
    //     {
    //         _name = value.SelectMany(b => b.Split(',',
    //             StringSplitOptions.RemoveEmptyEntries)).ToList();
    //     }
    // }
    //
    // private List<string> _description = [];
    // public List<string> Description
    // {
    //     get => _description;
    //     set
    //     {
    //         _description = value.SelectMany(b => b.Split(',',
    //             StringSplitOptions.RemoveEmptyEntries)).ToList();
    //     }
    // }
    public string? Sort { get; set; }
    
    private string? _search;
    public string Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }
}