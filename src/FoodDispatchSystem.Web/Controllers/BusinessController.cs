using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDispatchSystem.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class BusinessController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BusinessController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /Business
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var business = await _context.Businesses
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.IsConfigured);

            if (business == null)
            {
                return RedirectToAction("Index", "Setup");
            }

            var model = new BusinessEditViewModel
            {
                Id = business.Id,
                Name = business.Name,
                Address = business.Address,
                Phone = business.Phone,
                Email = business.Email,
                CurrencyCode = business.CurrencyCode,
                CurrentLogoPath = business.LogoPath
            };

            return View(model);
        }

        // POST: /Business
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BusinessEditViewModel model)
        {
            var business = await _context.Businesses
                .FirstOrDefaultAsync(b =>
                    b.Id == model.Id &&
                    b.IsConfigured);

            if (business == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.CurrentLogoPath = business.LogoPath;
                return View(model);
            }
            string? oldLogoPath = null;
            if (model.LogoFile != null &&
                model.LogoFile.Length > 0)
            {
                const long maxFileSize = 2 * 1024 * 1024;

                if (model.LogoFile.Length > maxFileSize)
                {
                    ModelState.AddModelError(
                        nameof(model.LogoFile),
                        "El logo no puede superar los 2 MB.");

                    model.CurrentLogoPath = business.LogoPath;
                    return View(model);
                }

                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };

                var extension = Path
                    .GetExtension(model.LogoFile.FileName)
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        nameof(model.LogoFile),
                        "Solo se permiten imágenes JPG, JPEG, PNG o WEBP.");

                    model.CurrentLogoPath = business.LogoPath;
                    return View(model);
                }

                var allowedContentTypes = new[]
                {
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                };

                if (!allowedContentTypes.Contains(
                    model.LogoFile.ContentType.ToLowerInvariant()))
                {
                    ModelState.AddModelError(
                        nameof(model.LogoFile),
                        "El archivo seleccionado no es una imagen válida.");

                    model.CurrentLogoPath = business.LogoPath;
                    return View(model);
                }

                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "business");

                Directory.CreateDirectory(uploadsFolder);

                var fileName =
                    $"{Guid.NewGuid():N}{extension}";

                var physicalPath = Path.Combine(
                    uploadsFolder,
                    fileName);

                await using (var stream =
                    new FileStream(
                        physicalPath,
                        FileMode.Create))
                {
                    await model.LogoFile.CopyToAsync(stream);
                }
                oldLogoPath = business.LogoPath;
                business.LogoPath =
                    $"/uploads/business/{fileName}";
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

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(oldLogoPath))
            {
                var oldFileName = Path.GetFileName(oldLogoPath);

                var oldPhysicalPath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "business",
                    oldFileName);

                if (System.IO.File.Exists(oldPhysicalPath))
                {
                    System.IO.File.Delete(oldPhysicalPath);
                }
            }

            TempData["SuccessMessage"] =
                "La información del negocio se actualizó correctamente.";

            return RedirectToAction(nameof(Index));
        }

    }
}