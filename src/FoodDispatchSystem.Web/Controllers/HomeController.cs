using FoodDispatchSystem.Web.Data;
using FoodDispatchSystem.Web.Models;
using FoodDispatchSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace FoodDispatchSystem.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var model = new DashboardViewModel
        {
            OrdersToday = await _context.Orders
                .CountAsync(o =>
                    o.CreatedAt >= today &&
                    o.CreatedAt < tomorrow),

            PendingOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Pending),

            PreparingOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Preparing),

            ReadyOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Ready),

            DeliveredToday = await _context.Orders
                .CountAsync(o =>
                    o.Status == OrderStatus.Delivered &&
                    o.CreatedAt >= today &&
                    o.CreatedAt < tomorrow),

            DeliveredTotalToday = await _context.Orders
                .Where(o =>
                    o.Status == OrderStatus.Delivered &&
                    o.CreatedAt >= today &&
                    o.CreatedAt < tomorrow)
                .SumAsync(o => (decimal?)o.Total) ?? 0,


                RecentOrders = await _context.Orders
    .Include(o => o.OrderDetails)
    .OrderByDescending(o => o.CreatedAt)
    .Take(5)
    .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id
                ?? HttpContext.TraceIdentifier
        });
    }
}