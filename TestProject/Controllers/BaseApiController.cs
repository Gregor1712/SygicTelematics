using System;
using AutoMapper;
using TestProject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TestProject.Entities;
using TestProject.RequestHelpers;

namespace TestProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private readonly IMapper _mapper;
    
    //private IMapper? _mapper;
    //protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
    protected BaseApiController()
    { }
    
    protected BaseApiController(IMapper mapper)
    {
        _mapper = mapper;
    }

    protected async Task<ActionResult> CreatePagedResultDto<T, TDto>(
        IGenericRepository<T> repository,
        ISpecification<T> specification,
        int pageIndex,
        int pageSize)
        where T : BaseEntity
        where TDto : class
    {
        var items = await repository.ListAsync(specification);
        var totalItems = await repository.CountAsync(specification);
        var data = _mapper.Map<IReadOnlyList<TDto>>(items);
        var pagination = new Pagination<TDto>(pageIndex, pageSize, totalItems, data);
        return Ok(pagination);
    }
    
    // protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> repository,
    //     ISpecification<T> specification, int pageIndex, int pageSize) where T : BaseEntity
    // {
    //     var items = await repository.ListAsync(specification);
    //     var totalItems = await repository.CountAsync(specification);
    //     var pagination = new Pagination<T>(pageIndex, pageSize, totalItems, items);
    //     return Ok(pagination);
    // }

    // protected async Task<ActionResult> CreatePagedResultDto<T, TDto>(IGenericRepository<T> repository,
    //     ISpecification<T> specification, int pageIndex, int pageSize)
    //     where T : BaseEntity//, IDtoConvertible
    //     where TDto: class
    // {
    //     var items = await repository.ListAsync(specification);
    //     var totalItems = await repository.CountAsync(specification);
    //     var data = _mapper.Map<IReadOnlyList<TDto>>(items);
    //     var pagination = new Pagination<TDto>(pageIndex, pageSize, totalItems, data);
    //     return Ok(pagination);
    // }
    
    // protected async Task<ActionResult> CreatePagedResult<T, TDto>(IGenericRepository<T> repo,
    //     ISpecification<T> spec, int pageIndex, int pageSize, Func<T, TDto> toDto)
    //     where T : BaseEntity, IDtoConvertible
    //     where TDto: class
    // {
    //     var items = await repo.ListAsync(spec);
    //     var count = await repo.CountAsync(spec);
    //
    //     var dtoItems = items.Select(toDto).ToList();
    //
    //     var pagination = new Pagination<TDto>(pageIndex, pageSize, count, dtoItems);
    //
    //     return Ok(pagination);
    // }
}