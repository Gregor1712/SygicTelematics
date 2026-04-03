using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestProject.DTO;
using TestProject.Entities;

namespace TestProject.Service;

public class SeedUsers
{
    public static async Task SeedUsersData(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var roles = new List<IdentityRole>
        {
            new() { Name = "User" },
            new() { Name = "Admin" }
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        var memberData = await File.ReadAllTextAsync("Csv/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (members == null)
        {
            Console.WriteLine("No members in seed data");
            return;
        }

        foreach (var member in members)
        {
            var user = new AppUser
            {
                Email = member.Email,
                UserName = member.Email,
                DisplayName = member.DisplayName,
            };

            var result = await userManager.CreateAsync(user, "User#123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
        }

        var admin = new AppUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
            DisplayName = "Admin"
        };

        var adminResult = await userManager.CreateAsync(admin, "Admin#123");
        if (adminResult.Succeeded)
        {
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "User" });
        }
        else
        {
            foreach (var error in adminResult.Errors)
            {
                Console.WriteLine(error.Description);
            }
        }
    }
}