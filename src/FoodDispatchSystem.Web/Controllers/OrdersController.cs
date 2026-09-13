using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDispatchSystem.Web.Models;
namespace FoodDispatchSystem.Web.Controllers;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Create()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Products = products;

        var model = new OrderCreateViewModel();

        model.Items.Add(new OrderItemInputModel
        {
            Quantity = 1
        });

        return View(model);

       
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(model);
        }
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            CreatedAt = DateTime.Now,
            Status = OrderStatus.Pending
        };
        decimal total = 0;

        foreach (var item in model.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == item.ProductId &&
                    p.IsActive);

            if (product == null)
            {
                ModelState.AddModelError(
                    "",
                    "Uno de los productos seleccionados no existe o está inactivo.");

                ViewBag.Products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return View(model);
            }

            var subtotal = product.Price * item.Quantity;

            var detail = new OrderDetail
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                Subtotal = subtotal
            };

            order.OrderDetails.Add(detail);

            total += subtotal;

        }


        order.Total = total;

        await using var transaction =
      await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Orders.Add(order);

            // Primer guardado:
            // SQL Server genera el Id del pedido.
            await _context.SaveChangesAsync();

            // Ahora que conocemos el Id, generamos el número visible.
            order.OrderNumber = $"ORD-{order.Id:D6}";

            // Segundo guardado:
            // actualizamos el número definitivo.
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["SuccessMessage"] =
                $"Pedido {order.OrderNumber} creado correctamente.";
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return RedirectToAction(nameof(Create));
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
     int id,
     OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        if (!Enum.IsDefined(typeof(OrderStatus), status))
        {
            return BadRequest();
        }

        var validTransition = order.Status switch
        {
            OrderStatus.Pending =>
                status == OrderStatus.Preparing ||
                status == OrderStatus.Cancelled,

            OrderStatus.Preparing =>
                status == OrderStatus.Ready ||
                status == OrderStatus.Cancelled,

            OrderStatus.Ready =>
                status == OrderStatus.Delivered,

            OrderStatus.Delivered => false,

            OrderStatus.Cancelled => false,

            _ => false
        };

        if (!validTransition)
        {
            TempData["ErrorMessage"] =
                "El cambio de estado solicitado no está permitido.";

            return RedirectToAction(nameof(Index));
        }

        order.Status = status;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}