using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FoodDispatchSystem.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.CategoryId = new SelectList(
            categories,
            "Id",
            "Name");

        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.CategoryId = new SelectList(
            categories,
            "Id",
            "Name",
            product.CategoryId);

        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var categories = await _context.Categories
            .Where(c => c.IsActive || c.Id == product.CategoryId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.CategoryId = new SelectList(
            categories,
            "Id",
            "Name",
            product.CategoryId);

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive || c.Id == product.CategoryId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.CategoryId = new SelectList(
                categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.IsActive = product.IsActive;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        product.IsActive = false;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reactivate(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        product.IsActive = true;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
