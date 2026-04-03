using Microsoft.AspNetCore.Mvc;
using TestProject.DTO;
using TestProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TestProject.Filters;
using TestProject.Wrappers;

[ApiController]
[Route("api/cpu")]
public class CpuController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public CpuController(ApplicationDbContext context)
    {
        _context = context;
    }

    //[Authorize(Policy = "RequireUserRole")]
    [HttpGet(Name = nameof(GetCpu))]
    public async Task<ActionResult<PagedResponse<List<CpuDTO>>>> GetCpu(
        [FromServices] IServerService server,
        [FromQuery] CpuFilter filter,
        [FromQuery] SortFilter sort,
        [FromQuery] PaginationFilter pagination)
    {
        var data =  await server.GetCPU(filter, sort, pagination);
        return Ok(data);
    }
    
    
    // GET: api/cpus/5
    // [HttpGet("{id}")]
    // public async Task<ActionResult<CpuDbo>> GetCPU(int id)
    // {
    //     var cpu = await _context.CPU
    //         .Include(c => c.Manufacturer)
    //         .FirstOrDefaultAsync(c => c.Id == id);
    //     
    //     if (cpu == null)
    //         return NotFound();
    //     
    //     return cpu;
    // }
    
    // GET: api/cpus/manufacturer/1
    // [HttpGet("manufacturer/{manufacturerId}")]
    // public async Task<ActionResult<IEnumerable<CpuDbo>>> GetCPUsByManufacturer(int manufacturerId)
    // {
    //     return await _context.CPU
    //         .Include(c => c.Manufacturer)
    //         .Where(c => c.ManufacturerDboId == manufacturerId)
    //         .ToListAsync();
    // }
    
    // POST: api/cpus
    // [HttpPost]
    // public async Task<ActionResult<CpuDbo>> CreateCPU(CpuDbo cpu)
    // {
    //     _context.CPU.Add(cpu);
    //     await _context.SaveChangesAsync();
    //     
    //     return CreatedAtAction(nameof(GetCPU), new { id = cpu.Id }, cpu);
    // }
    
    // PUT: api/cpus/5
    // [HttpPut("{id}")]
    // public async Task<IActionResult> UpdateCPU(int id, CpuDbo cpu)
    // {
    //     if (id != cpu.Id)
    //         return BadRequest();
    //     
    //     _context.Entry(cpu).State = EntityState.Modified;
    //     
    //     try
    //     {
    //         await _context.SaveChangesAsync();
    //     }
    //     catch (DbUpdateConcurrencyException)
    //     {
    //         if (!_context.CPU.Any(c => c.Id == id))
    //             return NotFound();
    //         throw;
    //     }
    //     
    //     return NoContent();
    // }
    
    // DELETE: api/cpus/5
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteCPU(int id)
    // {
    //     var cpu = await _context.CPU.FindAsync(id);
    //     if (cpu == null)
    //         return NotFound();
    //     
    //     _context.CPU.Remove(cpu);
    //     await _context.SaveChangesAsync();
    //     
    //     return NoContent();
    // }
}