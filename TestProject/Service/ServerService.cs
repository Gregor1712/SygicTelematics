using AutoMapper;
using TestProject.DTO;
using TestProject.Filters;
using TestProject.Interfaces;
using TestProject.Wrappers;

namespace TestProject.Service;

public class ServerService : IServerService
{
    private readonly ApplicationDbContext _context;
    private readonly ICpuService _cpuService;
    private readonly IMapper _mapper;
    
    public ServerService(ApplicationDbContext context,
        ICpuService cpuService,
        IMapper mapper)
    {
        _context = context;
        _cpuService = cpuService;
        _mapper = mapper;
    }
    
    public async Task<PagedResponse<IEnumerable<CpuDTO>>> GetCPU(CpuFilter filter, SortFilter sort, PaginationFilter pagination)
    {
        var result = await _cpuService.GetByFilter(filter, sort, pagination);
        var resultDto = _mapper.Map<IEnumerable<CpuDTO>>(result.Data);
        return new PagedResponse<IEnumerable<CpuDTO>>(resultDto, pagination, result.TotalRecords);
    }
}