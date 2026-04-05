using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Vehicle;
using Vehicle.Application.Entities;
using Vehicle.Application.Filters;
using Vehicle.Application.Interfaces;
using Vehicle.Infrastructure.Data;

namespace Vehicle.API.GrpcServices;

public class VehicleGrpcService(VehicleDbContext context, IServerService serverService)
    : VehicleGrpc.VehicleGrpcBase
{
    public override async Task<VehicleReply> GetById(VehicleIdRequest request, ServerCallContext ctx)
    {
        var id = Guid.Parse(request.VehicleId);
        var vehicle = await context.Vehicles.FindAsync(id);

        if (vehicle is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Vehicle not found"));

        return MapToReply(vehicle);
    }

    public override async Task<VehicleListReply> GetAll(GetVehiclesRequest request, ServerCallContext ctx)
    {
        var filter = new VehicleFilter();
        // StringCondition requires operator binding from query string, so we leave defaults
        // The gRPC GetAll is primarily for simple listing; complex filtering uses REST

        var sort = new SortFilter
        {
            SortProperty = request.HasSortBy ? request.SortBy : null,
            SortDirection = request.HasSortDirection
                ? Enum.TryParse<SortFilter.SortType>(request.SortDirection, true, out var dir) ? dir : null
                : null
        };

        var pagination = new PaginationFilter
        {
            PageNumber = request.PageNumber > 0 ? request.PageNumber : 1,
            PageSize = request.PageSize > 0 ? request.PageSize : 10
        };

        var data = await serverService.GetVehicle(filter, sort, pagination);

        var reply = new VehicleListReply
        {
            PageNumber = data.PageNumber,
            PageSize = data.PageSize,
            TotalRecords = data.TotalRecords
        };

        if (data.Data != null)
        {
            reply.Vehicles.AddRange(data.Data.Select(v => new VehicleReply
            {
                Id = v.Id.ToString(),
                Vin = v.Vin ?? "",
                Model = v.Model ?? "",
                Manufacturer = v.Manufacturer ?? "",
                Year = v.Year
            }));
        }

        return reply;
    }

    private static VehicleReply MapToReply(VehicleEntity e)
    {
        var reply = new VehicleReply
        {
            Id = e.Id.ToString(),
            Vin = e.Vin,
            Model = e.Model,
            Manufacturer = e.Manufacturer,
            Year = e.Year
        };

        if (e.CurrentLocationId.HasValue) reply.CurrentLocationId = e.CurrentLocationId.Value.ToString();
        if (e.BatteryStatusId.HasValue) reply.BatteryStatusId = e.BatteryStatusId.Value.ToString();
        if (e.LastTripId.HasValue) reply.LastTripId = e.LastTripId.Value.ToString();

        return reply;
    }
}
