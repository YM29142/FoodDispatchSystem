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
    public static async Task SeedCajeroAsync(
    IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var email = configuration["SeedCajero:Email"];
        var password = configuration["SeedCajero:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se configuraron las credenciales del cajero.");
        }

        var cajeroUser = await userManager.FindByEmailAsync(email);

        if (cajeroUser == null)
        {
            cajeroUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                cajeroUser,
                password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el cajero: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
            cajeroUser,
            "Cajero"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                cajeroUser,
                "Cajero");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo asignar el rol Cajero: {errors}");
            }
        }
    }
    public static async Task SeedCocinaAsync(
    IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var email = configuration["SeedCocina:Email"];
        var password = configuration["SeedCocina:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se configuraron las credenciales de Cocina.");
        }

        var cocinaUser = await userManager.FindByEmailAsync(email);

        if (cocinaUser == null)
        {
            cocinaUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                cocinaUser,
                password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el usuario Cocina: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
            cocinaUser,
            "Cocina"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                cocinaUser,
                "Cocina");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo asignar el rol Cocina: {errors}");
            }
        }
    }
    public static async Task SeedDespachoAsync(
    IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var email = configuration["SeedDespacho:Email"];
        var password = configuration["SeedDespacho:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se configuraron las credenciales de Despacho.");
        }

        var despachoUser = await userManager.FindByEmailAsync(email);

        if (despachoUser == null)
        {
            despachoUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                despachoUser,
                password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el usuario Despacho: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
            despachoUser,
            "Despacho"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                despachoUser,
                "Despacho");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo asignar el rol Despacho: {errors}");
            }
        }
    }
}