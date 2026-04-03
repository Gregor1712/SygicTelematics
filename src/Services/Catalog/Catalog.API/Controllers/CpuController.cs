using Catalog.Application.DTOs;
using Catalog.Application.Filters;
using Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Kernel.Wrappers;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/cpu")]
public class CpuController : ControllerBase
{
    [HttpGet(Name = nameof(GetCpu))]
    public async Task<ActionResult<PagedResponse<List<CpuDto>>>> GetCpu(
        [FromServices] IServerService server,
        [FromQuery] CpuFilter filter,
        [FromQuery] SortFilter sort,
        [FromQuery] PaginationFilter pagination)
    {
        var data = await server.GetCPU(filter, sort, pagination);
        return Ok(data);
    }
}