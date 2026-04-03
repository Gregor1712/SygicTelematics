using Catalog.Application.Filters.Conditions;

namespace Catalog.Application.Filters;

public class CpuFilter : FilterBase
{
    public StringCondition? Name { get; set; } = new(nameof(Name));
}