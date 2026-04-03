using Catalog.Application.Entities;
using Catalog.Application.Filters;
using Shared.Kernel.Wrappers;

namespace Catalog.Application.Interfaces;

public interface ICpuService
{
    public Task<DatabaseResult<IEnumerable<CpuDbo>>> GetByFilter(CpuFilter filter, SortFilter sort, PaginationFilter pagination);
}