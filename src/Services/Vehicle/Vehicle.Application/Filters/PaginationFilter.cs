using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.Filters;

public class PaginationFilter
{
    const int FIRST_PAGE_INDEX = 1;
    const int MAX_PAGE_SIZE = 500;

    [Range(FIRST_PAGE_INDEX, int.MaxValue)]
    public int? PageNumber { get; set; }

    [Range(0, MAX_PAGE_SIZE)]
    public int? PageSize { get; set; }

    public IQueryable<T> Apply<T>(IQueryable<T> query)
    {
        if (PageNumber == null || PageSize == null)
            return query;

        return query
            .Skip((PageNumber.Value - 1) * PageSize.Value)
            .Take(PageSize.Value);
    }
}
