using Shared.Kernel.Wrappers;
using Vehicle.Application.DTOs;
using Vehicle.Application.Filters;

namespace Vehicle.Application.Interfaces;

public interface IServerService
{
    Task<PagedResponse<List<VehicleDTO>>> GetVehicle(VehicleFilter filter, SortFilter sort, PaginationFilter pagination);
}
