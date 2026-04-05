using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/bff/alerts")]
public class BffAlertController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId)
    {
        var client = httpClientFactory.CreateClient("AlertService");
        var response = await client.GetAsync($"/api/alerts/vehicle/{vehicleId}");
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