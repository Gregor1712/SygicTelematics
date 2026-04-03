using Microsoft.AspNetCore.Mvc;
using TestProject.Dbo;
using TestProject.Filters;
using TestProject.Wrappers;

namespace TestProject.Interfaces;

public interface ICpuService
{
    //public DatabaseResult<ICollection<SkuskaDBO>> GetByFilter(SkuskaFilter filter, PaginationFilter pagination);
    public Task<DatabaseResult<IEnumerable<CpuDBO>>> GetByFilter(CpuFilter filter, SortFilter sort, PaginationFilter pagination);
    
    //public Task<IEnumerable<CpuDBO>> GetCPU();   
}