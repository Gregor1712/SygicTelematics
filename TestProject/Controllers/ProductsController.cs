using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestProject.Dbo;
using TestProject.DTO;
using TestProject.Interfaces;
using TestProject.RequestHelpers;
using TestProject.Specifications;

namespace TestProject.Controllers;

public class ProductsController(IUnitOfWork unit, IMapper mapper) : BaseApiController(mapper)
{
    [Authorize(Policy = "RequireUserRole")]
    [HttpGet]
    public async Task<ActionResult<Pagination<CpuDTO>>> GetProducts([FromQuery] ProductSpecParams productParams)
    {
        var spec = new ProductSpecification(productParams);
        //throw new ArgumentException(spec.ToString());
        return await CreatePagedResultDto<CpuDBO, CpuDTO>(
            unit.Repository<CpuDBO>(), spec, productParams.PageIndex, productParams.PageSize);
    }
    
    //[Cached(100000)]
    // [HttpGet]
    // public async Task<ActionResult<Pagination<CpuDBO>>> GetProducts([FromQuery]ProductSpecParams productParams)
    // {
    //     var spec = new ProductSpecification(productParams);
    //     return await CreatePagedResult(unit.Repository<CpuDBO>(), spec, productParams.PageIndex, productParams.PageSize);
    // } 
    
//     [HttpGet]
//     public async Task<ActionResult<Pagination<CpuDTO>>> GetProducts([FromQuery] ProductSpecParams productParams)
//     {
//         var spec = new ProductSpecification(productParams);
//         return await CreatePagedResultDto<CpuDBO, CpuDTO>(unit.Repository<CpuDBO>(), spec, productParams.PageIndex, productParams.PageSize);
//     } 

}

// public class ProductsController(IUnitOfWork unit, IMapper mapper) 
//     : BaseApiController(mapper)
// {
//     [HttpGet]
//     public async Task<ActionResult<Pagination<CpuDTO>>> GetProducts([FromQuery] ProductSpecParams productParams)
//     {
//         var spec = new ProductSpecification(productParams);
//         return await CreatePagedResultDto<CpuDBO, CpuDTO>(
//             unit.Repository<CpuDBO>(), spec, productParams.PageIndex, productParams.PageSize);
//     }
// }