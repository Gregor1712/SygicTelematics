using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Wrappers;
using Vehicle.Application.DTOs;
using Vehicle.Application.Filters;
using Vehicle.Application.Interfaces;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Services;

public class ServerService : IServerService
{
    private readonly VehicleDbContext _context;

    public ServerService(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<List<VehicleDTO>>> GetVehicle(
        VehicleFilter filter, SortFilter sort, PaginationFilter pagination)
    {
        var query = _context.Vehicles
            .AsNoTracking()
            .AsQueryable();

        query = filter.Apply(query);
        var totalRecords = await query.CountAsync();

        query = sort.Apply(query);
        query = pagination.Apply(query);

        var data = await query.Select(v => new VehicleDTO
        {
            Id = v.Id,
            Vin = v.Vin,
            Model = v.Model,
            Manufacturer = v.Manufacturer,
            Year = v.Year
        }).ToListAsync();

        return new PagedResponse<List<VehicleDTO>>(
            data,
            pagination.PageNumber ?? 1,
            pagination.PageSize ?? totalRecords,
            totalRecords);
    }
}
