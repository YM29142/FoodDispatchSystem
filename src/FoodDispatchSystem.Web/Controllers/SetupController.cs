using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Controllers
{
    [AllowAnonymous]
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SetupController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Setup
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var business = await _context.Businesses
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (business?.IsConfigured == true)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new BusinessSetupViewModel();

            if (business != null)
            {
                model.Name = business.Name;
                model.Address = business.Address;
                model.Phone = business.Phone;
                model.Email = business.Email;
                model.CurrencyCode = business.CurrencyCode;
            }

            return View(model);
        }

        // POST: /Setup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            BusinessSetupViewModel model)
        {
            var configuredBusiness = await _context.Businesses
                .AnyAsync(b => b.IsConfigured);

            if (configuredBusiness)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager
                .FindByEmailAsync(model.AdminEmail);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.AdminEmail),
                    "Ya existe un usuario con este correo electrónico.");

                return View(model);
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                const string adminRole = "Administrador";

                if (!await _roleManager.RoleExistsAsync(adminRole))
                {
                    var roleResult = await _roleManager.CreateAsync(
                        new IdentityRole(adminRole));

                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(
                                string.Empty,
                                error.Description);
                        }

                        await transaction.RollbackAsync();
                        return View(model);
                    }
                }

                var adminUser = new ApplicationUser
                {
                    UserName = model.AdminEmail.Trim(),
                    Email = model.AdminEmail.Trim(),
                    EmailConfirmed = true
                };

                var createUserResult =
                    await _userManager.CreateAsync(
                        adminUser,
                        model.AdminPassword);

                if (!createUserResult.Succeeded)
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(
                            nameof(model.AdminPassword),
                            error.Description);
                    }

                    await transaction.RollbackAsync();
                    return View(model);
                }

                var addRoleResult =
                    await _userManager.AddToRoleAsync(
                        adminUser,
                        adminRole);

                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    await transaction.RollbackAsync();
                    return View(model);
                }

                var business = await _context.Businesses
                    .FirstOrDefaultAsync();

                if (business == null)
                {
                    business = new Business();

                    _context.Businesses.Add(business);
                }

                business.Name = model.Name.Trim();

                business.Address =
                    string.IsNullOrWhiteSpace(model.Address)
                        ? null
                        : model.Address.Trim();

                business.Phone =
                    string.IsNullOrWhiteSpace(model.Phone)
                        ? null
                        : model.Phone.Trim();

                business.Email =
                    string.IsNullOrWhiteSpace(model.Email)
                        ? null
                        : model.Email.Trim();

                business.CurrencyCode = model.CurrencyCode
                    .Trim()
                    .ToUpperInvariant();

                business.IsConfigured = true;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return RedirectToAction(
                    "Login",
                    "Account");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}