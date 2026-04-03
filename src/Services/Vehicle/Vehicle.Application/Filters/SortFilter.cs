using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Vehicle.Application.Filters;

public class SortFilter
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortType
    {
        Ascending = 0,
        Descending
    }

    public string? SortProperty { get; set; }
    public SortType? SortDirection { get; set; } = SortType.Ascending;

    public IQueryable<T> Apply<T>(IQueryable<T> query)
    {
        if (string.IsNullOrEmpty(SortProperty))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, SortProperty);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = SortDirection == SortType.Ascending ? "OrderBy" : "OrderByDescending";

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    }
}
