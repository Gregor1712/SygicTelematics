using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Application.Filters;
using Catalog.Application.Interfaces;
using Shared.Kernel.Wrappers;

namespace Catalog.Infrastructure.Services;

public class ServerService : IServerService
{
    private readonly ICpuService _cpuService;
    private readonly IMapper _mapper;

    public ServerService(ICpuService cpuService, IMapper mapper)
    {
        _cpuService = cpuService;
        _mapper = mapper;
    }

    public async Task<PagedResponse<IEnumerable<CpuDto>>> GetCPU(CpuFilter filter, SortFilter sort, PaginationFilter pagination)
    {
        var result = await _cpuService.GetByFilter(filter, sort, pagination);
        var resultDto = _mapper.Map<IEnumerable<CpuDto>>(result.Data);
        return new PagedResponse<IEnumerable<CpuDto>>(
            resultDto,
            pagination.PageNumber ?? 1,
            pagination.PageSize ?? result.TotalRecords,
            result.TotalRecords);
    }
}