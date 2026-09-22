using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDispatchSystem.Web.Models;
namespace FoodDispatchSystem.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDispatchSystem.Web.Services;

[Authorize]
public class OrdersController : Controller

{
    private readonly ApplicationDbContext _context;
    private readonly OrderItemService _orderItemService;

    private readonly BusinessTimeService _businessTimeService;

    public OrdersController(
     ApplicationDbContext context,
     OrderItemService orderItemService,
     BusinessTimeService businessTimeService)
    {
        _context = context;
        _orderItemService = orderItemService;
        _businessTimeService = businessTimeService;
    }

    public async Task<IActionResult> Index(
      OrderStatus? status,
      bool today = false)
    {
        var query = _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }
        if (today)
        {
            var localToday = _businessTimeService.LocalToday;

            var (startOfTodayUtc, startOfTomorrowUtc) =
                _businessTimeService.GetUtcRangeForLocalDate(localToday);

            query = query.Where(o =>
                o.CreatedAt >= startOfTodayUtc &&
                o.CreatedAt < startOfTomorrowUtc);
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        ViewBag.SelectedStatus = status;
        ViewBag.TodayFilter = today;

        return View(orders);
    }

    [Authorize(Roles = "Administrador,Cajero")]
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
    [Authorize(Roles = "Administrador,Cajero")]
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
        var userId = User.FindFirstValue(
    ClaimTypes.NameIdentifier);

        var userEmail = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(userEmail))
        {
            return Forbid();
        }
        var utcNow = _businessTimeService.UtcNow;
        var localNow = _businessTimeService.ToLocalTime(utcNow);
        var order = new Order
        {
            OrderNumber = $"ORD-{localNow:yyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            CreatedAt = utcNow,
            Status = OrderStatus.Pending,

            CreatedByUserId = userId,
            CreatedByEmail = userEmail,
        };
        decimal total = 0;

        var consolidatedItems =
    _orderItemService.ConsolidateItems(model.Items);

        if (_orderItemService.ExceedsMaximumQuantity(model.Items))
        {
            ModelState.AddModelError(
                "",
                "La cantidad total de un producto no puede superar 100.");

            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(model);
        }
        foreach (var item in consolidatedItems)
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
                .ThenInclude(d => d.Product)
            .Include(o => o.StatusHistory)
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
        if (status == OrderStatus.Cancelled &&
             !User.IsInRole("Administrador") &&
            !User.IsInRole("Cajero"))
        {
            return Forbid();
        }

        if ((status == OrderStatus.Preparing ||
             status == OrderStatus.Ready) &&
             !User.IsInRole("Administrador") &&
             !User.IsInRole("Cocina"))
        {
            return Forbid();
        }

        if (status == OrderStatus.Delivered &&
            !User.IsInRole("Administrador") &&
            !User.IsInRole("Despacho"))
        {
            return Forbid();
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

        var previousStatus = order.Status;

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var userEmail = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(userEmail))
        {
            return Forbid();
        }

        var history = new OrderStatusHistory
        {
            OrderId = order.Id,
            PreviousStatus = previousStatus,
            NewStatus = status,
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = userId,
            ChangedByEmail = userEmail
        };

        _context.OrderStatusHistories.Add(history);

        order.Status = status;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}