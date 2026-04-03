using Catalog.Application.DTOs;
using Catalog.Application.Filters;
using Shared.Kernel.Wrappers;

namespace Catalog.Application.Interfaces;

public interface IServerService
{
    public Task<PagedResponse<IEnumerable<CpuDto>>> GetCPU(CpuFilter filter, SortFilter sort, PaginationFilter pagination);
}