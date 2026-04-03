using Identity.Application.DTOs;
using Identity.Application.Entities;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Extensions;

public static class AppUserExtensions
{
    public static async Task<UserDto> ToDto(this AppUser user, ITokenService tokenService,
        UserManager<AppUser> userManager)
    {
        var roles = await userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Token = await tokenService.CreateToken(user),
            Roles = roles
        };
    }
}