using Catalog.Application.Entities;
using Catalog.Application.Filters;
using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Wrappers;

namespace Catalog.Infrastructure.Services;

public class CpuService : ICpuService
{
    private readonly CatalogDbContext _context;

    public CpuService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<DatabaseResult<IEnumerable<CpuDbo>>> GetByFilter(
        CpuFilter filter,
        SortFilter sort,
        PaginationFilter pagination)
    {
        var query = _context.CPU
            .AsNoTracking()
            .AsQueryable();

        query = filter.Apply(query);
        var count = await query.CountAsync();

        query = sort.Apply(query);

        query = pagination.ApplyOrderById(query);
        var data = await query.ToListAsync();

        return new DatabaseResult<IEnumerable<CpuDbo>>
        {
            Data = data,
            TotalRecords = count
        };
    }
}