using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/account")]
public class BffAccountController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] JsonElement body)
    {
        var client = httpClientFactory.CreateClient("IdentityService");
        var content = new StringContent(body.GetRawText(), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/account/register", content);
        return await ForwardResponse(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] JsonElement body)
    {
        var client = httpClientFactory.CreateClient("IdentityService");
        var content = new StringContent(body.GetRawText(), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/account/login", content);

        // Forward Set-Cookie headers from Identity service
        ForwardCookies(response);

        return await ForwardResponse(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var client = httpClientFactory.CreateClient("IdentityService");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/refresh-token");

        // Forward the refresh token cookie to the Identity service
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            request.Headers.Add("Cookie", $"refreshToken={refreshToken}");
        }

        var response = await client.SendAsync(request);
        ForwardCookies(response);
        return await ForwardResponse(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var client = httpClientFactory.CreateClient("IdentityService");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/logout");

        // Forward the Authorization header
        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            request.Headers.Add("Authorization", authHeader.ToString());
        }

        var response = await client.SendAsync(request);
        ForwardCookies(response);
        return await ForwardResponse(response);
    }

    private async Task<IActionResult> ForwardResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            Content = responseContent,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }

    private void ForwardCookies(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                Response.Headers.Append("Set-Cookie", cookie);
            }
        }
    }
}