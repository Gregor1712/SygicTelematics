using Identity.Application.DTOs;
using Identity.Application.Entities;
using Identity.Application.Extensions;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
    UserManager<AppUser> userManager,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
        {
            return BadRequest("Email is already taken");
        }

        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await userManager.AddToRoleAsync(user, "User");

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService, userManager);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user is null)
        {
            return Unauthorized("Invalid email");
        }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result)
        {
            return Unauthorized("Invalid password");
        }

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService, userManager);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("No refresh token");
        }

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
                                      && x.RefreshTokenExpiry > DateTime.UtcNow);

        if (user is null)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService, userManager);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == User.GetMemberId());

        if (user is null)
        {
            return Unauthorized();
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await userManager.UpdateAsync(user);

        Response.Cookies.Delete("refreshToken");

        return Ok();
    }

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}