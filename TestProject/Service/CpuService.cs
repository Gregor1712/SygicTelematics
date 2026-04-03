using Microsoft.EntityFrameworkCore;
using TestProject.Dbo;
using TestProject.Filters;
using TestProject.Interfaces;
using TestProject.Wrappers;

namespace TestProject.Service;

public class CpuService : ICpuService
{
    private readonly ApplicationDbContext _context;

    public CpuService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<DatabaseResult<IEnumerable<CpuDBO>>> GetByFilter(
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
        //var data = await query.OrderByDescending(x => x.Id).ToListAsync();
        var data = await query.ToListAsync();
        
        return new DatabaseResult<IEnumerable<CpuDBO>>(data, count);
    }
    
    // public async Task<IEnumerable<CpuDBO>> GetCPU()
    // {
    //     return await _context.CPU
    //         .Include(c => c.Manufacturer)
    //         .ToListAsync();
    // }
}