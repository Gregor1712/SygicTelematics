using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/battery")]
public class BffBatteryController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var client = httpClientFactory.CreateClient("BatteryService");
        var response = await client.GetAsync($"/api/battery/vehicle/{vehicleId}");
        return await ForwardResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JsonElement body)
    {
        var client = httpClientFactory.CreateClient("BatteryService");
        var content = new StringContent(body.GetRawText(), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/battery", content);
        return await ForwardResponse(response);
    }

    private static async Task<IActionResult> ForwardResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            Content = responseContent,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }
}
