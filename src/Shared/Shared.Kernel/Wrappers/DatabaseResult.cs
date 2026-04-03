namespace Shared.Kernel.Wrappers;

public class DatabaseResult<T>
{
    public required T Data { get; set; }
    public int TotalRecords { get; set; }
}