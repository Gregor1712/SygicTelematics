using Gateway.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Alert;
using Shared.Grpc.Battery;
using Shared.Grpc.Location;
using Shared.Grpc.Telemetry;
using Shared.Grpc.Trip;
using Shared.Grpc.Vehicle;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/vehicles")]
public class BffVehicleController(
    VehicleGrpc.VehicleGrpcClient vehicleClient,
    LocationGrpc.LocationGrpcClient locationClient,
    BatteryGrpc.BatteryGrpcClient batteryClient,
    TripGrpc.TripGrpcClient tripClient,
    TelemetryGrpc.TelemetryGrpcClient telemetryClient,
    AlertGrpc.AlertGrpcClient alertClient,
    IHttpClientFactory httpClientFactory) : ControllerBase
{
    /// <summary>
    /// Get aggregated vehicle detail with location, battery, last trip, telemetry and alerts via gRPC
    /// </summary>
    [HttpGet("{vehicleId:guid}")]
    public async Task<IActionResult> GetVehicleDetail(Guid vehicleId)
    {
        var vehicleIdStr = vehicleId.ToString();

        // Call all services in parallel via gRPC
        var vehicleTask = vehicleClient.GetByIdAsync(
            new Shared.Grpc.Vehicle.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var locationTask = locationClient.GetByVehicleAsync(
            new Shared.Grpc.Location.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var batteryTask = batteryClient.GetByVehicleAsync(
            new Shared.Grpc.Battery.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var tripTask = tripClient.GetByVehicleAsync(
            new Shared.Grpc.Trip.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var telemetryTask = telemetryClient.GetByVehicleAsync(
            new Shared.Grpc.Telemetry.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;
        var alertTask = alertClient.GetByVehicleAsync(
            new Shared.Grpc.Alert.VehicleIdRequest { VehicleId = vehicleIdStr }).ResponseAsync;

        await Task.WhenAll(vehicleTask, locationTask, batteryTask, tripTask, telemetryTask, alertTask);

        var vehicle = await vehicleTask;
        var locations = await locationTask;
        var battery = await batteryTask;
        var trips = await tripTask;
        var telemetry = await telemetryTask;
        var alerts = await alertTask;

        return this.ProtoJsonComposite(
            ("vehicle", vehicle),
            ("location", locations.Locations),
            ("battery", battery.Statuses),
            ("trips", trips.Trips),
            ("telemetry", telemetry.Records),
            ("alerts", alerts.Alerts));
    }

    /// <summary>
    /// Get all vehicles with filtering, sorting and pagination (HTTP forwarding for complex query binding)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllVehicles()
    {
        var client = httpClientFactory.CreateClient("VehicleService");
        var queryString = Request.QueryString.Value ?? "";
        var response = await client.GetAsync($"/api/vehicles{queryString}");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            Content = content,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }
}
