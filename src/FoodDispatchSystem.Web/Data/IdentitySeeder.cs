using FoodDispatchSystem.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodDispatchSystem.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        {
            "Administrador",
            "Cajero",
            "Cocina",
            "Despacho"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(role));
            }
        }
    }
    public static async Task SeedAdminAsync(
    IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se configuraron las credenciales del administrador.");
        }

        var adminUser = await userManager.FindByEmailAsync(email);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                adminUser,
                password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el administrador: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
            adminUser,
            "Administrador"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                "Administrador");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo asignar el rol Administrador: {errors}");
            }
        }
    }
}