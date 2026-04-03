using TestProject.DTO;
using TestProject.Filters;
using TestProject.Wrappers;

namespace TestProject.Interfaces;

public interface IServerService
{
    public Task<PagedResponse<IEnumerable<CpuDTO>>> GetCPU(CpuFilter filter, SortFilter sort, PaginationFilter pagination);
}