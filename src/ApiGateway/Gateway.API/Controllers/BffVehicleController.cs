using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/vehicles")]
public class BffVehicleController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Get aggregated vehicle detail with location, battery, last trip, telemetry and alerts
    /// </summary>
    [HttpGet("{vehicleId:guid}")]
    public async Task<IActionResult> GetVehicleDetail(Guid vehicleId)
    {
        var vehicleClient = httpClientFactory.CreateClient("VehicleService");
        var locationClient = httpClientFactory.CreateClient("LocationService");
        var batteryClient = httpClientFactory.CreateClient("BatteryService");
        var tripClient = httpClientFactory.CreateClient("TripService");
        var telemetryClient = httpClientFactory.CreateClient("TelemetryService");
        var alertClient = httpClientFactory.CreateClient("AlertService");

        // Call all services in parallel
        var vehicleTask = vehicleClient.GetAsync($"/api/vehicles/{vehicleId}");
        var locationTask = locationClient.GetAsync($"/api/locations/vehicle/{vehicleId}");
        var batteryTask = batteryClient.GetAsync($"/api/battery/vehicle/{vehicleId}");
        var tripTask = tripClient.GetAsync($"/api/trips/vehicle/{vehicleId}");
        var telemetryTask = telemetryClient.GetAsync($"/api/telemetry/vehicle/{vehicleId}");
        var alertTask = alertClient.GetAsync($"/api/alerts/vehicle/{vehicleId}");

        await Task.WhenAll(vehicleTask, locationTask, batteryTask, tripTask, telemetryTask, alertTask);

        var vehicleResponse = await vehicleTask;
        if (!vehicleResponse.IsSuccessStatusCode)
            return NotFound(new { message = "Vehicle not found" });

        var vehicle = await ReadJson(vehicleResponse);
        var locations = await ReadJson(await locationTask);
        var batteryStatuses = await ReadJson(await batteryTask);
        var trips = await ReadJson(await tripTask);
        var telemetryRecords = await ReadJson(await telemetryTask);
        var alerts = await ReadJson(await alertTask);

        var result = new
        {
            vehicle,
            location = locations,
            battery = batteryStatuses,
            trips,
            telemetry = telemetryRecords,
            alerts
        };

        return Ok(result);
    }

    /// <summary>
    /// Get all vehicles with their latest location and battery status
    /// </summary>
    [Authorize(Policy = "RequireUserRole")]
    [HttpGet]
    public async Task<IActionResult> GetAllVehicles()
    {
        var vehicleClient = httpClientFactory.CreateClient("VehicleService");
        var queryString = Request.QueryString.Value ?? "";
        var response = await vehicleClient.GetAsync($"/api/vehicles{queryString}");

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

    private static async Task<JsonElement?> ReadJson(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content)) return null;

        return JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
    }
}