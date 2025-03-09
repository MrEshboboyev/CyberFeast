using BuildingBlocks.Abstractions.Persistence;
using FoodDelivery.Services.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodDelivery.Services.Identity.Identity.Data;

public class IdentityDataSeeder(
    UserManager<ApplicationUser> userManager, 
    RoleManager<ApplicationRole> roleManager) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    public int Order => 1;

    private async Task SeedRoles()
    {
        if (!await roleManager.RoleExistsAsync(ApplicationRole.Admin.Name!))
            await roleManager.CreateAsync(ApplicationRole.Admin);

        if (!await roleManager.RoleExistsAsync(ApplicationRole.User.Name!))
            await roleManager.CreateAsync(ApplicationRole.User);
    }

    private async Task SeedUsers()
    {
        if (await userManager.FindByEmailAsync("mreshboboyev@test.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "MrEshboboyev",
                FirstName = "MrEshboboyev",
                LastName = "test",
                Email = "mreshboboyev@test.com",
            };

            var result = await userManager.CreateAsync(user, "123456");

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, ApplicationRole.Admin.Name!);
        }

        if (await userManager.FindByEmailAsync("mreshboboyev2@test.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "MrEshboboyev2",
                FirstName = "MrEshboboyev",
                LastName = "Test",
                Email = "mreshboboyev2@test.com"
            };

            var result = await userManager.CreateAsync(user, "123456");

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, ApplicationRole.User.Name!);
        }
    }
}