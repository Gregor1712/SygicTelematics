using System.Text.Json;
using Identity.Application.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public static class SeedUsers
{
    public static async Task SeedUsersData(UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var roles = new List<string> { "User", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users is null) return;

        foreach (var user in users)
        {
            user.UserName = user.Email;
            user.EmailConfirmed = true;

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }

        var admin = new AppUser
        {
            DisplayName = "Admin",
            UserName = "admin@test.com",
            Email = "admin@test.com",
            EmailConfirmed = true
        };

        var adminResult = await userManager.CreateAsync(admin, "Pa$$w0rd");

        if (adminResult.Succeeded)
        {
            await userManager.AddToRolesAsync(admin, ["User", "Admin"]);
        }
    }
}