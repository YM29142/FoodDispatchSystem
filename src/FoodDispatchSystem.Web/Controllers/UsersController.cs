using FoodDispatchSystem.Web.Models;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .ToListAsync();

            var model = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager
                    .GetRolesAsync(user);

                var role = roles.FirstOrDefault()
                    ?? "Sin rol";

                var isLocked =
                    user.LockoutEnd.HasValue &&
                    user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                model.Add(new UserListViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Role = role,
                    IsActive = !isLocked
                });
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            UserCreateViewModel model)
        {
            var allowedRoles = new[]
            {
        "Cajero",
        "Cocina",
        "Despacho"
    };

            if (!allowedRoles.Contains(model.Role))
            {
                ModelState.AddModelError(
                    nameof(model.Role),
                    "El rol seleccionado no es válido.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim();

            var existingUser = await _userManager
                .FindByEmailAsync(normalizedEmail);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Ya existe un usuario con este correo electrónico.");

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(
                user,
                model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        nameof(model.Password),
                        error.Description);
                }

                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                model.Role);

            if (!roleResult.Succeeded)
            {
                // Evitamos dejar un usuario creado sin rol.
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            TempData["SuccessMessage"] =
                $"El empleado {normalizedEmail} fue creado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUserId =
                _userManager.GetUserId(User);

            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "No puede desactivar su propia cuenta.";

                return RedirectToAction(nameof(Index));
            }

            if (await _userManager.IsInRoleAsync(
                user,
                "Administrador"))
            {
                TempData["ErrorMessage"] =
                    "No se puede desactivar una cuenta de Administrador desde esta opción.";

                return RedirectToAction(nameof(Index));
            }

            await _userManager.SetLockoutEnabledAsync(
                user,
                true);

            var result =
                await _userManager.SetLockoutEndDateAsync(
                    user,
                    DateTimeOffset.MaxValue);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] =
                    "No fue posible desactivar el usuario.";

                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] =
                $"El empleado {user.Email} fue desactivado.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result =
                await _userManager.SetLockoutEndDateAsync(
                    user,
                    null);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] =
                    "No fue posible activar el usuario.";

                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] =
                $"El empleado {user.Email} fue activado.";

            return RedirectToAction(nameof(Index));
        }
    }


}
